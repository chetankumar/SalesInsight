namespace RedHill.SalesInsight.Web.Html5.Models
{
    public class DistrictListView
    {
        public string Name;
        public int DistrictId;
        public bool IsSelected;

        public DistrictListView(DAL.District d,bool selected)
        {
            this.Name = d.Name;
            this.DistrictId = d.DistrictId;
            this.IsSelected = selected;
        }
    }
}