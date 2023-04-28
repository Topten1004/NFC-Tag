/*
 Navicat Premium Data Transfer

 Source Server         : MySQL
 Source Server Type    : MySQL
 Source Server Version : 100424
 Source Host           : localhost:3306
 Source Schema         : clinotag-bqs

 Target Server Type    : MySQL
 Target Server Version : 100424
 File Encoding         : 65001

 Date: 26/04/2023 03:48:46
*/

SET NAMES utf8mb4;
SET FOREIGN_KEY_CHECKS = 0;

-- ----------------------------
-- Table structure for __efmigrationshistory
-- ----------------------------
DROP TABLE IF EXISTS `__efmigrationshistory`;
CREATE TABLE `__efmigrationshistory`  (
  `MigrationId` varchar(150) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NOT NULL,
  `ProductVersion` varchar(32) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NOT NULL,
  PRIMARY KEY (`MigrationId`) USING BTREE
) ENGINE = InnoDB CHARACTER SET = utf8mb4 COLLATE = utf8mb4_general_ci ROW_FORMAT = Dynamic;

-- ----------------------------
-- Records of __efmigrationshistory
-- ----------------------------
INSERT INTO `__efmigrationshistory` VALUES ('20230426072017_First', '6.0.1');

-- ----------------------------
-- Table structure for agent
-- ----------------------------
DROP TABLE IF EXISTS `agent`;
CREATE TABLE `agent`  (
  `ID_AGENT` int(11) NOT NULL AUTO_INCREMENT,
  `NOM` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NOT NULL,
  `CODE` char(5) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NOT NULL,
  PRIMARY KEY (`ID_AGENT`) USING BTREE,
  UNIQUE INDEX `AgentCodeUnique`(`CODE`) USING BTREE
) ENGINE = InnoDB AUTO_INCREMENT = 12 CHARACTER SET = utf8mb4 COLLATE = utf8mb4_general_ci ROW_FORMAT = Dynamic;

-- ----------------------------
-- Records of agent
-- ----------------------------
INSERT INTO `agent` VALUES (11, 'Agent 0', '00000');

-- ----------------------------
-- Table structure for client
-- ----------------------------
DROP TABLE IF EXISTS `client`;
CREATE TABLE `client`  (
  `ID_CLIENT` int(11) NOT NULL AUTO_INCREMENT,
  `NOM` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NOT NULL,
  PRIMARY KEY (`ID_CLIENT`) USING BTREE
) ENGINE = InnoDB AUTO_INCREMENT = 10 CHARACTER SET = utf8mb4 COLLATE = utf8mb4_general_ci ROW_FORMAT = Dynamic;

-- ----------------------------
-- Records of client
-- ----------------------------
INSERT INTO `client` VALUES (9, 'CHS');

-- ----------------------------
-- Table structure for geoloc_agent
-- ----------------------------
DROP TABLE IF EXISTS `geoloc_agent`;
CREATE TABLE `geoloc_agent`  (
  `ID_GEOLOC_AGENT` int(11) NOT NULL AUTO_INCREMENT,
  `ID_CONSTRUCTEUR` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NOT NULL,
  `ID_AGENT` int(11) NOT NULL,
  `LATI` double NOT NULL,
  `LONGI` double NOT NULL,
  `DH_GEOLOC` datetime(6) NOT NULL,
  `IP_GEOLOC` varchar(30) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NOT NULL,
  PRIMARY KEY (`ID_GEOLOC_AGENT`) USING BTREE,
  INDEX `IX_GEOLOC_AGENT_ID_AGENT`(`ID_AGENT`) USING BTREE,
  CONSTRAINT `FK_GEOLOC_AGENT_UTILISATEUR` FOREIGN KEY (`ID_AGENT`) REFERENCES `agent` (`ID_AGENT`) ON DELETE RESTRICT ON UPDATE RESTRICT
) ENGINE = InnoDB AUTO_INCREMENT = 1045 CHARACTER SET = utf8mb4 COLLATE = utf8mb4_general_ci ROW_FORMAT = Dynamic;

-- ----------------------------
-- Records of geoloc_agent
-- ----------------------------
INSERT INTO `geoloc_agent` VALUES (1038, '0103760291063104219NZ', 11, -22.2943458557129, 166.455078125, '2023-01-03 12:27:56.360000', '175.158.163.185');
INSERT INTO `geoloc_agent` VALUES (1039, '0103760291063104219NZ', 11, -22.294246673584, 166.455230712891, '2023-01-03 12:28:24.880000', '175.158.163.185');
INSERT INTO `geoloc_agent` VALUES (1040, '0103760291063104219NZ', 11, -22.294246673584, 166.455230712891, '2023-01-03 12:28:31.733000', '175.158.163.185');
INSERT INTO `geoloc_agent` VALUES (1041, '0103760291063104219NZ', 11, -22.2942562103271, 166.455230712891, '2023-01-03 12:28:36.153000', '175.158.163.185');
INSERT INTO `geoloc_agent` VALUES (1042, '0103760291063104219NZ', 11, -22.2942562103271, 166.455230712891, '2023-01-03 12:28:45.417000', '175.158.163.185');
INSERT INTO `geoloc_agent` VALUES (1043, '0103760291063104219NZ', 11, -22.2942981719971, 166.455184936523, '2023-01-03 12:28:55.923000', '175.158.163.185');
INSERT INTO `geoloc_agent` VALUES (1044, '0103760291063104219NZ', 11, -22.2940158843994, 166.455001831055, '2023-01-03 12:32:32.377000', '175.158.163.185');

-- ----------------------------
-- Table structure for lieu
-- ----------------------------
DROP TABLE IF EXISTS `lieu`;
CREATE TABLE `lieu`  (
  `ID_LIEU` int(11) NOT NULL AUTO_INCREMENT,
  `NOM` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NOT NULL,
  `ID_CLIENT` int(11) NOT NULL,
  `UID_TAG` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NOT NULL,
  `ACTION_TYPE` int(11) NOT NULL,
  `PROGRESS` int(11) NOT NULL,
  PRIMARY KEY (`ID_LIEU`) USING BTREE,
  UNIQUE INDEX `TagUniqueLieu`(`UID_TAG`) USING BTREE,
  INDEX `IX_LIEU_ID_CLIENT`(`ID_CLIENT`) USING BTREE,
  CONSTRAINT `FK_LIEU_CLIENT` FOREIGN KEY (`ID_CLIENT`) REFERENCES `client` (`ID_CLIENT`) ON DELETE RESTRICT ON UPDATE RESTRICT
) ENGINE = InnoDB AUTO_INCREMENT = 36 CHARACTER SET = utf8mb4 COLLATE = utf8mb4_general_ci ROW_FORMAT = Dynamic;

-- ----------------------------
-- Records of lieu
-- ----------------------------
INSERT INTO `lieu` VALUES (23, ' Bât B - Sanitaire', 9, '53DE708A01F440', 0, 0);
INSERT INTO `lieu` VALUES (24, 'Bât B - Bureau Directrice Adjointe', 9, '530604AB01F040', 0, 0);
INSERT INTO `lieu` VALUES (25, 'Bât B - Bureau 2', 9, '534E4583017040', 0, 0);
INSERT INTO `lieu` VALUES (26, 'Bât B - Bureau 3', 9, '535615A901DF40', 0, 0);
INSERT INTO `lieu` VALUES (27, 'Bât B - Bureau 4', 9, '5306648901D840', 0, 0);
INSERT INTO `lieu` VALUES (28, 'Bât B - Secrétariat', 9, '534E65A3016840', 0, 0);
INSERT INTO `lieu` VALUES (29, 'Bât B - Bureau 5', 9, '5356358B01F840', 0, 0);
INSERT INTO `lieu` VALUES (30, 'Bât B - Bureau 6', 9, '538F112C014240', 0, 0);
INSERT INTO `lieu` VALUES (31, 'Bât B - Bureau 7', 9, '5356442C013D40', 0, 0);
INSERT INTO `lieu` VALUES (32, 'Bât B - Archives', 9, '530E312C01C540', 0, 0);
INSERT INTO `lieu` VALUES (33, 'Bât B - Office/Cuisine', 9, '531F010701E140', 0, 0);
INSERT INTO `lieu` VALUES (34, 'Bât B - Salle De Réunion', 9, '5357200E017540', 0, 0);
INSERT INTO `lieu` VALUES (35, 'Bât B - Entrée Sortie', 9, '530F100F018140', 0, 0);

-- ----------------------------
-- Table structure for materiel
-- ----------------------------
DROP TABLE IF EXISTS `materiel`;
CREATE TABLE `materiel`  (
  `ID_MATERIEL` int(11) NOT NULL AUTO_INCREMENT,
  `NOM` varchar(150) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NOT NULL,
  `INSTRUCTION` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NULL,
  `ID_CLIENT` int(11) NOT NULL,
  `UID_TAG` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NOT NULL,
  `EXPIRATION` int(11) NOT NULL,
  PRIMARY KEY (`ID_MATERIEL`) USING BTREE,
  UNIQUE INDEX `TagUniqueObjet`(`UID_TAG`) USING BTREE,
  INDEX `IX_MATERIEL_ID_CLIENT`(`ID_CLIENT`) USING BTREE,
  CONSTRAINT `FK_MATERIEL_CLIENT` FOREIGN KEY (`ID_CLIENT`) REFERENCES `client` (`ID_CLIENT`) ON DELETE RESTRICT ON UPDATE RESTRICT
) ENGINE = InnoDB AUTO_INCREMENT = 1 CHARACTER SET = utf8mb4 COLLATE = utf8mb4_general_ci ROW_FORMAT = Dynamic;

-- ----------------------------
-- Table structure for notification
-- ----------------------------
DROP TABLE IF EXISTS `notification`;
CREATE TABLE `notification`  (
  `ID_NOTIFICATION` int(11) NOT NULL AUTO_INCREMENT,
  `ID_UTILISATION` int(11) NOT NULL,
  `DH_NOTIFICATION` datetime(6) NOT NULL,
  `TYPE_DESTINATAIRE` varchar(10) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NOT NULL,
  PRIMARY KEY (`ID_NOTIFICATION`) USING BTREE,
  INDEX `IX_NOTIFICATION_ID_UTILISATION`(`ID_UTILISATION`) USING BTREE,
  CONSTRAINT `FK_NOTIFICATION_UTILISATION` FOREIGN KEY (`ID_UTILISATION`) REFERENCES `utilisation` (`ID_UTILISATION`) ON DELETE RESTRICT ON UPDATE RESTRICT
) ENGINE = InnoDB AUTO_INCREMENT = 1 CHARACTER SET = utf8mb4 COLLATE = utf8mb4_general_ci ROW_FORMAT = Dynamic;

-- ----------------------------
-- Table structure for passage
-- ----------------------------
DROP TABLE IF EXISTS `passage`;
CREATE TABLE `passage`  (
  `ID_PASSAGE` int(11) NOT NULL AUTO_INCREMENT,
  `ID_LIEU` int(11) NOT NULL,
  `ID_AGENT` int(11) NOT NULL,
  `DH_DEBUT` datetime(6) NOT NULL,
  `DH_FIN` datetime(6) NOT NULL,
  `PHOTO` longblob NULL,
  `COMMENTAIRE` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NULL,
  PRIMARY KEY (`ID_PASSAGE`) USING BTREE,
  INDEX `IX_PASSAGE_ID_AGENT`(`ID_AGENT`) USING BTREE,
  INDEX `IX_PASSAGE_ID_LIEU`(`ID_LIEU`) USING BTREE,
  CONSTRAINT `FK_PASSAGE_LIEU` FOREIGN KEY (`ID_LIEU`) REFERENCES `lieu` (`ID_LIEU`) ON DELETE RESTRICT ON UPDATE RESTRICT,
  CONSTRAINT `FK_PASSAGE_PASSAGE` FOREIGN KEY (`ID_AGENT`) REFERENCES `agent` (`ID_AGENT`) ON DELETE RESTRICT ON UPDATE RESTRICT
) ENGINE = InnoDB AUTO_INCREMENT = 1 CHARACTER SET = utf8mb4 COLLATE = utf8mb4_general_ci ROW_FORMAT = Dynamic;

-- ----------------------------
-- Table structure for passage_tache
-- ----------------------------
DROP TABLE IF EXISTS `passage_tache`;
CREATE TABLE `passage_tache`  (
  `ID_PT` int(11) NOT NULL AUTO_INCREMENT,
  `ID_PASSAGE` int(11) NOT NULL,
  `ID_TL` int(11) NOT NULL,
  `FAIT` tinyint(1) NOT NULL,
  PRIMARY KEY (`ID_PT`) USING BTREE,
  INDEX `IX_PASSAGE_TACHE_ID_PASSAGE`(`ID_PASSAGE`) USING BTREE,
  INDEX `IX_PASSAGE_TACHE_ID_TL`(`ID_TL`) USING BTREE,
  CONSTRAINT `FK_PASSAGE_TACHE_PASSAGE` FOREIGN KEY (`ID_PASSAGE`) REFERENCES `passage` (`ID_PASSAGE`) ON DELETE CASCADE ON UPDATE RESTRICT,
  CONSTRAINT `FK_PASSAGE_TACHE_TACHE_LIEU` FOREIGN KEY (`ID_TL`) REFERENCES `tache_lieu` (`ID_TL`) ON DELETE CASCADE ON UPDATE RESTRICT
) ENGINE = InnoDB AUTO_INCREMENT = 1 CHARACTER SET = utf8mb4 COLLATE = utf8mb4_general_ci ROW_FORMAT = Dynamic;

-- ----------------------------
-- Table structure for role
-- ----------------------------
DROP TABLE IF EXISTS `role`;
CREATE TABLE `role`  (
  `ROLE` varchar(10) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NOT NULL,
  PRIMARY KEY (`ROLE`) USING BTREE
) ENGINE = InnoDB CHARACTER SET = utf8mb4 COLLATE = utf8mb4_general_ci ROW_FORMAT = Dynamic;

-- ----------------------------
-- Records of role
-- ----------------------------
INSERT INTO `role` VALUES ('ADMIN');
INSERT INTO `role` VALUES ('MANAGER');
INSERT INTO `role` VALUES ('SUPERADMIN');

-- ----------------------------
-- Table structure for tache
-- ----------------------------
DROP TABLE IF EXISTS `tache`;
CREATE TABLE `tache`  (
  `ID_TACHE` int(11) NOT NULL AUTO_INCREMENT,
  `NOM` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NOT NULL,
  `DESCRIPTION` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NULL,
  PRIMARY KEY (`ID_TACHE`) USING BTREE
) ENGINE = InnoDB AUTO_INCREMENT = 1 CHARACTER SET = utf8mb4 COLLATE = utf8mb4_general_ci ROW_FORMAT = Dynamic;

-- ----------------------------
-- Table structure for tache_lieu
-- ----------------------------
DROP TABLE IF EXISTS `tache_lieu`;
CREATE TABLE `tache_lieu`  (
  `ID_TL` int(11) NOT NULL AUTO_INCREMENT,
  `ID_TACHE` int(11) NOT NULL,
  `ID_LIEU` int(11) NOT NULL,
  PRIMARY KEY (`ID_TL`) USING BTREE,
  UNIQUE INDEX `IX_TACHE_LIEU`(`ID_LIEU`, `ID_TACHE`) USING BTREE,
  INDEX `IX_TACHE_LIEU_ID_TACHE`(`ID_TACHE`) USING BTREE,
  CONSTRAINT `FK_TACHE_LIEU_LIEU` FOREIGN KEY (`ID_LIEU`) REFERENCES `lieu` (`ID_LIEU`) ON DELETE CASCADE ON UPDATE RESTRICT,
  CONSTRAINT `FK_TACHE_LIEU_TACHE` FOREIGN KEY (`ID_TACHE`) REFERENCES `tache` (`ID_TACHE`) ON DELETE RESTRICT ON UPDATE RESTRICT
) ENGINE = InnoDB AUTO_INCREMENT = 1 CHARACTER SET = utf8mb4 COLLATE = utf8mb4_general_ci ROW_FORMAT = Dynamic;

-- ----------------------------
-- Table structure for tache_planifiee
-- ----------------------------
DROP TABLE IF EXISTS `tache_planifiee`;
CREATE TABLE `tache_planifiee`  (
  `ID_TACHE_PLANIFIEE` int(11) NOT NULL AUTO_INCREMENT,
  `TACHE_PLANIFIEE_ACTIVE` tinyint(1) NOT NULL,
  `ACTION_TACHE_PLANIFIEE` varchar(20) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NOT NULL,
  `CRONTAB` varchar(30) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NOT NULL,
  `DESCRIPTION_CRONTAB` varchar(120) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NULL DEFAULT NULL,
  `DH_TACHE_PLANIFIEE` datetime(6) NULL DEFAULT NULL,
  `DH_DERNIERE_TACHE` datetime(6) NULL DEFAULT NULL,
  `TACHE_ACCOMPLIE` tinyint(1) NOT NULL,
  PRIMARY KEY (`ID_TACHE_PLANIFIEE`) USING BTREE
) ENGINE = InnoDB AUTO_INCREMENT = 1 CHARACTER SET = utf8mb4 COLLATE = utf8mb4_general_ci ROW_FORMAT = Dynamic;

-- ----------------------------
-- Table structure for uclient
-- ----------------------------
DROP TABLE IF EXISTS `uclient`;
CREATE TABLE `uclient`  (
  `ID_UCLIENT` int(11) NOT NULL AUTO_INCREMENT,
  `ID_UTILISATEUR` int(11) NOT NULL,
  `ID_CLIENT` int(11) NOT NULL,
  PRIMARY KEY (`ID_UCLIENT`) USING BTREE,
  UNIQUE INDEX `NonClusteredIndex-UtilisateurClient`(`ID_UTILISATEUR`, `ID_CLIENT`) USING BTREE,
  INDEX `IX_UCLIENT_ID_CLIENT`(`ID_CLIENT`) USING BTREE,
  CONSTRAINT `FK_UCLIENT_CLIENT` FOREIGN KEY (`ID_CLIENT`) REFERENCES `client` (`ID_CLIENT`) ON DELETE RESTRICT ON UPDATE RESTRICT,
  CONSTRAINT `FK_UCLIENT_UTILISATEUR` FOREIGN KEY (`ID_UTILISATEUR`) REFERENCES `utilisateur` (`ID_UTILISATEUR`) ON DELETE RESTRICT ON UPDATE RESTRICT
) ENGINE = InnoDB AUTO_INCREMENT = 22 CHARACTER SET = utf8mb4 COLLATE = utf8mb4_general_ci ROW_FORMAT = Dynamic;

-- ----------------------------
-- Records of uclient
-- ----------------------------
INSERT INTO `uclient` VALUES (21, 7, 9);

-- ----------------------------
-- Table structure for utilisateur
-- ----------------------------
DROP TABLE IF EXISTS `utilisateur`;
CREATE TABLE `utilisateur`  (
  `ID_UTILISATEUR` int(11) NOT NULL AUTO_INCREMENT,
  `NOM` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NOT NULL,
  `PRENOM` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NOT NULL,
  `LOGIN` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NOT NULL,
  `EMAIL` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NOT NULL,
  `MDP` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NOT NULL,
  `ROLE` varchar(10) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NOT NULL,
  PRIMARY KEY (`ID_UTILISATEUR`) USING BTREE,
  INDEX `IX_UTILISATEUR_ROLE`(`ROLE`) USING BTREE,
  CONSTRAINT `FK_UTILISATEUR_ROLE` FOREIGN KEY (`ROLE`) REFERENCES `role` (`ROLE`) ON DELETE RESTRICT ON UPDATE RESTRICT
) ENGINE = InnoDB AUTO_INCREMENT = 10 CHARACTER SET = utf8mb4 COLLATE = utf8mb4_general_ci ROW_FORMAT = Dynamic;

-- ----------------------------
-- Records of utilisateur
-- ----------------------------
INSERT INTO `utilisateur` VALUES (6, 'Admin', 'Istrateur', 'admin', 'grool.sarl@gmail.com', 'Admin8', 'SUPERADMIN');
INSERT INTO `utilisateur` VALUES (7, 'Masuyer', 'Cédric', 'cedric', 'c.masuyer@bqs.nc', 'clinotag2023', 'ADMIN');
INSERT INTO `utilisateur` VALUES (9, 'Chs', 'Nouville', 'chs', 'clinotag@chs.nc', 'chs2023', 'MANAGER');

-- ----------------------------
-- Table structure for utilisation
-- ----------------------------
DROP TABLE IF EXISTS `utilisation`;
CREATE TABLE `utilisation`  (
  `ID_UTILISATION` int(11) NOT NULL AUTO_INCREMENT,
  `DH_DEBUT` datetime(6) NOT NULL,
  `DH_FIN` datetime(6) NULL DEFAULT NULL,
  `ID_MATERIEL` int(11) NOT NULL,
  `ID_AGENT` int(11) NOT NULL,
  `COMMENTAIRE` varchar(250) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NULL DEFAULT NULL,
  `CLOTURE` varchar(10) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NULL DEFAULT NULL,
  PRIMARY KEY (`ID_UTILISATION`) USING BTREE,
  INDEX `IX_UTILISATION_ID_AGENT`(`ID_AGENT`) USING BTREE,
  INDEX `IX_UTILISATION_ID_MATERIEL`(`ID_MATERIEL`) USING BTREE,
  CONSTRAINT `FK_UTILISATION_AGENT` FOREIGN KEY (`ID_AGENT`) REFERENCES `agent` (`ID_AGENT`) ON DELETE RESTRICT ON UPDATE RESTRICT,
  CONSTRAINT `FK_UTILISATION_MATERIEL` FOREIGN KEY (`ID_MATERIEL`) REFERENCES `materiel` (`ID_MATERIEL`) ON DELETE RESTRICT ON UPDATE RESTRICT
) ENGINE = InnoDB AUTO_INCREMENT = 1 CHARACTER SET = utf8mb4 COLLATE = utf8mb4_general_ci ROW_FORMAT = Dynamic;

SET FOREIGN_KEY_CHECKS = 1;
