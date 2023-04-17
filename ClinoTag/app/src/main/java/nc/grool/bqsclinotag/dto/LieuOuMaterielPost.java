package nc.grool.bqsclinotag.dto;

import java.util.List;

public class LieuOuMaterielPost {

    public int idClient;
    public String nom;
    public String uidTag;

    public LieuOuMaterielPost(String _uidTag, int _idClient, String _nom) {
        idClient = _idClient;
        uidTag = _uidTag;
        nom = _nom;
    }
}