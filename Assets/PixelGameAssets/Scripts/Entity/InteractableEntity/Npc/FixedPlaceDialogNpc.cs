using PixelGameAssets.Scripts.Actor;
using TMPro;
using Random = System.Random;

namespace PixelGameAssets.Scripts.Entity.InteractableEntity.Npc
{
    public class FixedPlaceDialogNpc : InteractableNpc
    {
        
        public string[] content;

        public string[] voiceName;
        
        protected new void Awake()
        {

            foreach (var s in content)
            {
                Content.Add(new Statement(Modal.Normal, s));
            }

            foreach (var voice in voiceName)
            {
                NpcVoiceName.Add(voice);
            }
            
//            npcName = "真正的勇者";
//            Content.Add(new Statement(Modal.Normal, "你好"));
//            Content.Add(new Statement(Modal.Normal, "F**k"));
//            Content.Add(new Statement(Modal.Normal, "老子是勇者"));
//            Content.Add(new Statement(Modal.Normal, "欢迎来到**"));
//            Content.Add(new Statement(Modal.Normal, "WTF??!!"));
//            NpcVoiceName.Add("嗯_1");
//            NpcVoiceName.Add("嗯_2");
//            NpcVoiceName.Add("嗯_3");
            
            base.Awake();
        }
        
        protected new void Update()
        {
            base.Update();
        }

        protected override void OnPlayerTrigger(BasePlayer commonPlayer)
        {
            if (Dialog == null) return;
            base.OnPlayerTrigger(commonPlayer);

            var statement = Content[new Random().Next(0, Content.Count)];

            if (statement.Modal == Modal.Normal)
            {
                var text = Dialog.GetComponentInChildren<TextMeshProUGUI>();
                text.text = statement.Content;
            }
            else
            {
                // TODO: 其他语气对话框
            }
            
            PlayRandomVoice();
        }
    }
}