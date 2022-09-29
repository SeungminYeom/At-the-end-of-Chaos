using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhotonErrorHandler : MonoBehaviour
{
    private static PhotonErrorHandler instance = null;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        } else
        {
            Destroy(this.gameObject);
        }
    }

    public static PhotonErrorHandler Instance
    {
        get { return instance; }
    }

    public string Err(short errCode)
    {
        switch (errCode)
        {
            case -3:
                return "현재 상태에서 허용되지 않은 작업";

            case -2:
                return "알 수 없는 오류";

            case -1:
                return "Internal Server Error";

            case 32762:
                return "서버가 가득찼습니다.";

            case 32766:
                return "해당 ID는 이미 사용중입니다.";

            case 32752:
                return "Plugin Error";

            case 32751:
                return "Plugin Mismatch";

            case 32742:
                return "슬롯 에러";

            case 32765:
                return "인원 초과";

            case 32764:
                return "해당 게임은 종료되었습니다.";

            case 32758:
                return "해당 번호의 게임은 존재하지 않습니다.";

            case 32750:
                return "이미 접속하였습니다.";

            case 32749:
                return "비활화된 인원이 존재합니다.";

            case 32748:
                return "재접속 가능한 플레이어를 찾지 못하였습니다.";

            case 32747:
                return "Found Excluded UserId";
 
            case 32746:
                return "Found Active Joiner";

            case 32760:
                return "해당 번호의 게임이 존재하지 않습니다.";

            default:
                return "-";
        }
    }
}
