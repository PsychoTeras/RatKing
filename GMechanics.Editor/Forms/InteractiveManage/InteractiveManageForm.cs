using System.Windows.Forms;
using GMechanics.Core.Classes.Entities;
using GMechanics.Core.Classes.EntitiesLists;
using GMechanics.Core.Classes.Enums;

namespace GMechanics.Editor.Forms.InteractiveManage
{
    public partial class InteractiveManageForm : Form
    {
        private Atom _atom;
        private GameEntityType _gameEntityType;
        private readonly InteractiveRecipientsList _clonedList;

        public InteractiveRecipientsList Result
        {
            get { return _clonedList; }
        }

        public InteractiveManageForm(Atom atom, GameEntityType gameEntityType)
        {
            InitializeComponent();

            _atom = atom;
            _gameEntityType = gameEntityType;
            _clonedList = ((InteractiveRecipientsList) atom.InteractiveRecipientsData).Clone();
        }
    }
}
