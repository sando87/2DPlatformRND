using UnityEngine;

namespace PahlBit
{
    public class PopupManager : SingletonMono<PopupManager>
    {
        [SerializeField] Transform RootCanvas = null;

        PopupBase[] mPopupPrefabs = null;

        void Start()
        {
            mPopupPrefabs = Resources.LoadAll<PopupBase>("Prefabs/Popups");
        }

        public T Show<T>() where T : PopupBase
        {
            // 미리 등록된 팝업 프리팹을 검색하여 찾다가 해당 컴포넌트를 가지고 있는 팝업이 있다면 팝업 생성
            foreach(PopupBase prefab in mPopupPrefabs)
            {
                if(prefab.GetComponent<T>() != null)
                {
                    // 팝업 생성
                    PopupBase popup = Instantiate(prefab, RootCanvas);
                    return popup as T;
                }
            }
            LOG.warn();
            return null;
        }

        public void Close(PopupBase popup)
        {
            Destroy(popup.gameObject);
        }

        public T Toggle<T>() where T : PopupBase
        {
            // 이미 열려있는 팝업이 있는지 검색
            foreach (Transform child in RootCanvas)
            {
                T popup = child.GetComponent<T>();
                if (popup != null)
                {
                    // 팝업이 열려있다면 닫기
                    Close(popup);
                    return null;
                }
            }
            // 팝업이 열려있지 않다면 팝업 생성
            return Show<T>();
        }
    }

    
}