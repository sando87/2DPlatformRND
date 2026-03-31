using UnityEngine;

namespace PahlBit
{
    public class PopupStats : PopupBase
    {
        [SerializeField] GameObject StaticsRowPrefab;
        [SerializeField] Transform ContentsRoot;
        
        void Start()
        {
            UpdateUIParts();
            
            foreach (var btn in mUIParts)
            {
                btn.EventSelect.AddListener(() => {
                    LOG.trace(btn.name);
                });
                // btn.EventDeselect.AddListener(() => {
                //     LOG.trace(btn.name);
                // });
                btn.EventSubmit.AddListener(() => {
                    LOG.trace(btn.name);
                });
            }
        }
    }
}