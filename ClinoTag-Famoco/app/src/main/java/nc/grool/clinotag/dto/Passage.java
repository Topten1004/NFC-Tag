package nc.grool.clinotag.dto;

import java.util.Date;
import java.util.List;

public class Passage {

    public int idPassage;
    public String dhDebut;
    public String dhFin;
    public int idLieu;
    public int idAgent;
    public String commentaire;
    public String photo;
    public List<Tache> lTache;

    public Date dateDebut;
}
