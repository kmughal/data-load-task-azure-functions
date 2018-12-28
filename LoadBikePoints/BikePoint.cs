namespace LoadBikePoints
{
    using Microsoft.WindowsAzure.Storage.Table;
    using System.Runtime.Serialization;

    public class BikePoint : TableEntity
    {
        [DataMember]
        public string Id { get; set; }

        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public string TerminalName { set; get; }

        [DataMember]
        public decimal Lat { set; get; }

        [DataMember]
        public decimal Long { get; set; }

        [DataMember]
        public bool Installed { get; set; }

        [DataMember]
        public bool Locked { get; set; }

        [DataMember]
        public string InstallDate { get; set; }

        [DataMember]
        public string Temporary { get; set; }

        [DataMember]
        public int NbBikes { get; set; }

        [DataMember]
        public int NbEmptyDocks { get; set; }

        [DataMember]
        public int NbDocks { get; set; }


        public BikePoint()
        {
            PartitionKey = "BikePoint";
            RowKey = Name;
        }
    }
}
