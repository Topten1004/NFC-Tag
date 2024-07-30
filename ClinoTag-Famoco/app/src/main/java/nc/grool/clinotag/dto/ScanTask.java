package nc.grool.clinotag.dto;

import java.util.List;

public class ScanTask {
    public String name ;

    public String description;

    public List<Byte[]> imageData;


    public ScanTask(String _name, String _description, List<Byte[]> _imageData)
    {
        this.name = _name;
        this.description = _description;
        this.imageData = _imageData;
    }
}
