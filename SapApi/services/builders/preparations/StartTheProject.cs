using SAP2000v1;
using System.IO;
using System.Reflection;
using System.Windows.Forms;

namespace SAP2000.services.builders.preparations
{
    class StartTheProject
    {
        private cSapModel _sapModel;
        public StartTheProject(cSapModel sapModel)
        {
            _sapModel = sapModel;
        }


        public void CreateNewModel()
        {
            var assembly = Assembly.GetExecutingAssembly();
            string resourceName = "SAP2000.Init.sdb";
            string tempFilePath = Path.Combine(Path.GetTempPath(), "temp_sap_model.sdb");
            using (Stream stream = assembly.GetManifestResourceStream(resourceName))
            {
                if (stream == null)
                {
                    MessageBox.Show($"Kaynak bulunamadı: {resourceName}");
                    return;                }

                using (FileStream fileStream = new FileStream(tempFilePath, FileMode.Create))
                {
                    stream.CopyTo(fileStream);
                }
            }

            _sapModel.File.OpenFile(tempFilePath);
        }
    }
}
