using System.Windows.Forms;

namespace SAP2000.services.validators
{
    public interface IInputValidator
    {
        bool validate(Control control, out string errorMessage);
        void applyValidationStyle(Control control, bool isValid);
    }
}
