namespace EmployeeManagement.ViewModels
{
    public class HomeEditViewModel : HomeCreateViewModel
    {
        public int ID { get; set; }

        public string ExistingPhotoPath { get; set; }
    }
}
