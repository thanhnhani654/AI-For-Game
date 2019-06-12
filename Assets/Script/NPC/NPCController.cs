using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCController : MonoBehaviour {

    public enum eNPCState { Idle, FollowPath, FireTarget, FollowTarget }
    public enum eNPCFireState { Stand, MoveStraight, MoveLR, Pursuit, MoveToCoverPoint, ReturnLastPoint }
    public enum eNPCLevel { Easy, Normal, Hard }

    public Weapon.eWeaponType weaponType;
    public Weapon weapon;
    public Sprite[] sprites;

    FieldOfView fow;
    NPCRotateToTarget npcRoTTarget;
    // Tầm Nhìn của NPC
    public float radius = 3.0f;
    public LayerMask mask;

    public eNPCLevel level = eNPCLevel.Easy;

    //FireState
    NPCFire npcFire; // Hình như không dùng tới

    // Thời gian chuyển trạng thái của NPC
    public float nextStateTime = 1.0f;
    float nextStateTimeCount;
    // Dùng để chọn ngẫu nhiên hướng di chuyển cho trạng thái di chuyển lúc bắn của NPC
    public float randomRadius = 5;

    // Lưu trữ hướng di chuyển cho trạng thái di chuyển lúc bắn của NPC
    Vector3 directionMS;

    public LayerMask obstacleMask;
    // Khoảng cách để NPC không tiến lại quá gần mục tiêu
    public float avoidTargetRadius = 5;

    // True khi không tìm thấy mục tiêu trong trạng thái tìm mục tiêu
    bool lostTarget;
    // True khi mục tiêu mất khỏi tầm nhìn và chuyển sang trạng thái tìm mục tiêu
    bool tempLostTarget;
    // Thời gian để quyết định mất mục tiêu để quay về trạng thái Tìm kiếm 
    public float timeLostTarget = 5;
    public float timeLostTargetCount;
    // Lưu trữ tọa độ lúc mục tiêu mất khỏi tầm nhìn
    Vector3 LostLocation;

    public eNPCState state = eNPCState.FollowPath;

    public eNPCFireState fireState = eNPCFireState.Stand;

    public NPCPathFinder pathFinder;
    public GameObject dataGrid;
    public GameObject debugPointToGo;
    public bool debugPathFinding = true;

    // Lưu trữ thông số NPC
    Attribute stat;
    // True để Spawn lần đầu
    public bool directSpawn;

    public bool pause = false;
    // Tọa độ xác định hướng nhìn của NPC. trong trường hợp NPC di chuyển không theo hướng
    // nhìn thì là tọa độ của NPC trong frame kế tiếp
    public Vector3 nextPosition;                                     
    public Vector3 velocity;        // Hình như không dùng tới
    float speedScale;

    float countBugTime = 0;

    // Use this for initialization
    void Start() {
        fow = GetComponent<FieldOfView>();
        npcFire = GetComponent<NPCFire>();
        npcRoTTarget = GetComponent<NPCRotateToTarget>();
        pathFinder = new NPCPathFinder(dataGrid, debugPointToGo);
        stat = GetComponent<Attribute>();
        pathFinder.debug = debugPathFinding;
        nextStateTimeCount = nextStateTime;
        timeLostTargetCount = timeLostTarget;

        //GetWeapon
        weapon = this.transform.GetComponentInChildren<Weapon>();
        weapon.Owner = this.gameObject;
        weapon.SetWeaponWithType(weaponType);
        ChangeSprite(weaponType);
    }

    // Update is called once per frame
    void Update() {
        if (Input.GetKeyDown(KeyCode.Escape))
            pause = !pause;

        if (pause)
            return;

        speedScale = 1;

        //Function IsLostTarget()//////////////////
        if (npcRoTTarget.target != null)
        {
            lostTarget = false;
            tempLostTarget = false;
            timeLostTargetCount = timeLostTarget;
        }
        else
        {
            tempLostTarget = true;
        }

        // Khi vào trạng thái theo dấu mục tiêu(FollowTarget) thì bắt đầu đếm thời gian để quyết định mất dấu mục tiêu
        if (!lostTarget && tempLostTarget)
        {
            timeLostTargetCount -= Time.deltaTime;
        }

        if (timeLostTargetCount < 0)
            lostTarget = true;
        ///////////////////////////////////////////

        // Đoạn này chắc đọc code hiểu thoy
        switch (state)
        {
            case eNPCState.Idle:
                break;
            case eNPCState.FollowPath:
                if (npcRoTTarget.target != null)
                {
                    state = eNPCState.FireTarget;
                }

                nextPosition = pathFinder.FollowPath(this.transform.position, stat.speed,obstacleMask);

                velocity = (nextPosition - transform.position).normalized;

                
                if (pathFinder.bReachPoint)
                    pathFinder.FindNewPath(this.transform.position);

                //Wandering
                npcRoTTarget.RotateToPosition(nextPosition);

                break;
            case eNPCState.FollowTarget:
                if (lostTarget)
                    state = eNPCState.FollowPath;

                if (!tempLostTarget)
                    state = eNPCState.FireTarget;

                nextPosition = pathFinder.FollowPath(this.transform.position, LostLocation, stat.speed,obstacleMask);

                velocity = (nextPosition - transform.position).normalized;

                if (pathFinder.bReachPoint)
                {
                    timeLostTargetCount = -1;
                    pathFinder.FindNewPath(this.transform.position);
                }

                //npcRoTTarget.RotateToPosition(nextPosition);

                break;
            case eNPCState.FireTarget:
                if (tempLostTarget)
                {
                    if (level == eNPCLevel.Easy)
                        state = eNPCState.FollowPath;
                    else
                        state = eNPCState.FollowTarget;
                }

                if (npcRoTTarget.target != null)
                    LostLocation = npcRoTTarget.target.transform.position;

                UpdateFireState();

                break;
        }

        if (state != eNPCState.FireTarget)
        {
            ObstacleAvoidance();
            npcRoTTarget.RotateToPosition(nextPosition);
            this.transform.position += transform.TransformDirection(Vector3.up) * stat.speed;
        }
        else
        {
            this.transform.position = nextPosition;
        }
       
    }

    void UpdateFireState()
    {
        //Weapon
        npcRoTTarget.RotateToTarget();
        // Thực hiện việc bắn
        weapon.Fire();


        if (level == eNPCLevel.Easy)
            return;

        switch (fireState)
        {
            case eNPCFireState.Stand:
                if (nextStateTimeCount < 0)
                {
                    nextStateTimeCount = nextStateTime / 4;
                    fireState = RandomNextState();
                }
                nextStateTimeCount -= Time.deltaTime;

                if (level == eNPCLevel.Hard)
                {
                    nextStateTimeCount = nextStateTime;
                    fireState = eNPCFireState.MoveLR;
                }

                //if (npcFire.bReloading)
                //    fireState = eNPCFireState.MoveToCoverPoint;              

                break;
            case eNPCFireState.MoveStraight:        // Di chuyển theo hướng đã xác định

                if (nextStateTimeCount < 0)
                {
                    nextStateTimeCount = nextStateTime;
                    fireState = RandomNextState();
                }
                nextStateTimeCount -= Time.deltaTime;

                if (Physics2D.Raycast(this.transform.position, (directionMS - this.transform.position).normalized, 1, obstacleMask).collider == null)
                {
                    nextPosition = Vector3.MoveTowards(this.transform.position, directionMS, stat.speed);

                    if (level == eNPCLevel.Hard && this.transform.position == directionMS)
                    {
                        nextStateTimeCount = nextStateTime;
                        fireState = eNPCFireState.MoveLR;
                    }
                }

                break;
              
            case eNPCFireState.MoveLR:          // Xác định hướng ngẫu nhiên để di chuyển

                if (nextStateTimeCount < 0)
                {
                    nextStateTimeCount = nextStateTime;
                    fireState = RandomNextState();
                }
                nextStateTimeCount -= Time.deltaTime;

                if (Vector3.Distance(this.transform.position, directionMS) < 1)
                {
                    GetDirectionMS();
                    nextStateTimeCount = nextStateTime;
                    fireState = eNPCFireState.MoveStraight;
                }

                break;

                if (Physics2D.Raycast(this.transform.position, (directionMS - this.transform.position).normalized, 1, obstacleMask).collider == null)
                {
                    this.transform.position = Vector3.MoveTowards(this.transform.position, directionMS, stat.speed);
                }

                //if (npcFire.bReloading)
                //    fireState = eNPCFireState.MoveToCoverPoint;

                //if standtimecount < 0 then randmnextState
                break;
            case eNPCFireState.Pursuit:
                //if Reaching target point && target is visible then random next state
                //else target not visible then followPath
                break;
            case eNPCFireState.MoveToCoverPoint:
                //if Reaching Cover Point then
                // ReturnLastPoint;

                break;
            case eNPCFireState.ReturnLastPoint:
                // if ReachLastPoint then
                //      if target is visible then random next state
                //      else state = pursuit
                break;
        }
    }

    // Ráng làm cho lúc bắn nó ảo tí mà chưa được
    eNPCFireState RandomNextState()
    {
        int t = Random.Range(1, 3);

        if (t == 1)
        {
            GetDirectionMS();
        }

        return (eNPCFireState)t;
    }

    // Xác định hướng di chuyển lúc bắn
    void GetDirectionMS()
    {
        if (npcRoTTarget.target == null)
            return;
        int count = 0;
        while (count < 100)
        {
            count++;
            Vector3 dir = npcRoTTarget.target.position - this.transform.position;
            dir.Normalize();
            if (count < 100)
            {
                dir.x += Random.Range(-randomRadius, randomRadius);
                dir.y += Random.Range(-randomRadius, randomRadius);
            }
            else
            {
                dir.x -= randomRadius;      //De chac chan NPC tranh xa nhau khi o qua gan
                dir.y -= randomRadius;
            }

            directionMS = dir + this.transform.position;

            if (Vector3.Distance(directionMS, npcRoTTarget.target.position) > avoidTargetRadius)
                break;
        }
    }

    // ReSpawn
    public bool ReSpawn()
    {
        if (stat == null)
            return false;
        stat.ReSpawn();

        state = eNPCState.FollowPath;
        pathFinder.bReachPoint = true;
        pathFinder.FindNewPath(this.transform.position);

        switch (Random.Range(1, 3))
        {
            case 1:
                weaponType = Weapon.eWeaponType.Gun;               
                break;
            case 2:
                weaponType = Weapon.eWeaponType.Rifle;
                break;
            case 3:
                weaponType = Weapon.eWeaponType.Shotgun;
                break;
        }

        weapon.SetWeaponWithType(weaponType);
        ChangeSprite(weaponType);
        npcRoTTarget.target = null;

        return true;
    }

    // Thuật toán tránh va chạm 
    // Tạo ra 3 ray bắn ra 3 hướng trước mặt
    // 2 hướng left và right dùng để khi phát hiện trước mặt có tường hoặc npc thì nó né sang hướng còn lại
    // ray ở giữa dùng để giảm tốc độ lại
    void ObstacleAvoidance()
    {
        Vector3 leftDirect = new Vector3(Vector3.up.x - 0.4f, Vector3.up.y, 0);
        Vector3 rightDirect = new Vector3(Vector3.up.x + 0.4f, Vector3.up.y, 0);
                
        Debug.DrawRay(this.transform.position, transform.TransformDirection(Vector3.right*2), Color.magenta);

        RaycastHit2D center = Physics2D.Raycast(this.transform.position, transform.TransformDirection(Vector3.up), 3);
        RaycastHit2D left = Physics2D.Raycast(this.transform.position, transform.TransformDirection(leftDirect), 3);
        RaycastHit2D right = Physics2D.Raycast(this.transform.position, transform.TransformDirection(rightDirect), 3);

        if (center.collider != null)
        {
            Debug.DrawRay(this.transform.position, transform.TransformDirection(Vector3.up * 3), Color.red);
        }
        else
        {
            Debug.DrawRay(this.transform.position, transform.TransformDirection(Vector3.up * 3), Color.green);
        }

        if (left.collider != null)
        {
            Debug.DrawRay(this.transform.position, transform.TransformDirection(leftDirect * 2), Color.red);
            nextPosition += transform.TransformDirection(Vector3.right) * stat.speed;
        }
        else
        {
            Debug.DrawRay(this.transform.position, transform.TransformDirection(leftDirect * 2), Color.green);
        }

        if (right.collider != null)
        {
            nextPosition -= transform.TransformDirection(Vector3.right) * stat.speed;
            Debug.DrawRay(this.transform.position, transform.TransformDirection(rightDirect * 2), Color.red);
        }
        else
        {
            Debug.DrawRay(this.transform.position, transform.TransformDirection(rightDirect * 2), Color.green);
        }

        if (center.collider != null && left.collider != null && right.collider != null)
        {
            countBugTime += Time.deltaTime;
            if (countBugTime >= 0.5f)
            {
                countBugTime = 0;
                pathFinder.bReachPoint = true;
            }
        }
        else
        {
            countBugTime = 0;
        }
        
    }

    // Quyết định Team1 thì màu xanh, 2 thì đỏ, cầm súng gì thì hình ra sao
    void ChangeSprite(Weapon.eWeaponType type)
    {
        switch (type)
        {
            case Weapon.eWeaponType.Gun:
                if (stat.Team == Attribute.eTeam.Team1)
                    this.GetComponent<SpriteRenderer>().sprite = sprites[0];
                else
                    this.GetComponent<SpriteRenderer>().sprite = sprites[3];
                break;
            case Weapon.eWeaponType.Rifle:
                if (stat.Team == Attribute.eTeam.Team1)
                    this.GetComponent<SpriteRenderer>().sprite = sprites[1];
                else
                    this.GetComponent<SpriteRenderer>().sprite = sprites[4];
                break;
            case Weapon.eWeaponType.Shotgun:
                if (stat.Team == Attribute.eTeam.Team1)
                    this.GetComponent<SpriteRenderer>().sprite = sprites[2];
                else
                    this.GetComponent<SpriteRenderer>().sprite = sprites[5];
                break;
        }
        
    }
}
