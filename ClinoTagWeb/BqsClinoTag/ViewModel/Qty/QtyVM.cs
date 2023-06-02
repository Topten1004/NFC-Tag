using System.ComponentModel.DataAnnotations;

namespace BqsClinoTag.ViewModel.Qty
{
    public class QtyVM
    {
        public QtyVM() {
            datas = new List<QtyVMList>();
        }
        public List<QtyVMList> datas { get; set; }
    }

    public class QtyVMList
    {
        public int count { get; set; }

        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime Created { get; set; }

        public string Name { get; set; } = string.Empty!;

    }
}
