/*
 Navicat Premium Data Transfer

 Source Server         : MySQL
 Source Server Type    : MySQL
 Source Server Version : 100424
 Source Host           : localhost:3306
 Source Schema         : clinotag

 Target Server Type    : MySQL
 Target Server Version : 100424
 File Encoding         : 65001

 Date: 15/04/2023 01:32:35
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
INSERT INTO `__efmigrationshistory` VALUES ('20230415052456_First', '6.0.1');
INSERT INTO `__efmigrationshistory` VALUES ('20230415052653_clino', '6.0.1');

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
) ENGINE = InnoDB AUTO_INCREMENT = 6 CHARACTER SET = utf8mb4 COLLATE = utf8mb4_general_ci ROW_FORMAT = Dynamic;

-- ----------------------------
-- Records of agent
-- ----------------------------
INSERT INTO `agent` VALUES (1, 'John Doe', '12333');
INSERT INTO `agent` VALUES (2, 'Jane Dae', '12777');
INSERT INTO `agent` VALUES (5, 'Aude Javel', '66666');

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
INSERT INTO `client` VALUES (1, 'SLN');
INSERT INTO `client` VALUES (2, 'OPT');
INSERT INTO `client` VALUES (3, 'Le Froid');
INSERT INTO `client` VALUES (4, 'Enercal');
INSERT INTO `client` VALUES (5, 'CHS');
INSERT INTO `client` VALUES (6, 'CH101#TWIN#105');
INSERT INTO `client` VALUES (7, 'CH102#SGL#75');
INSERT INTO `client` VALUES (8, 'CH103#TRIPLE#155');
INSERT INTO `client` VALUES (9, 'CH104#SUITE#400');

-- ----------------------------
-- Table structure for lieu
-- ----------------------------
DROP TABLE IF EXISTS `lieu`;
CREATE TABLE `lieu`  (
  `ID_LIEU` int(11) NOT NULL AUTO_INCREMENT,
  `NOM` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NOT NULL,
  `ID_CLIENT` int(11) NOT NULL,
  `UID_TAG` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NOT NULL,
  PRIMARY KEY (`ID_LIEU`) USING BTREE,
  INDEX `IX_LIEU_ID_CLIENT`(`ID_CLIENT`) USING BTREE,
  CONSTRAINT `FK_LIEU_CLIENT` FOREIGN KEY (`ID_CLIENT`) REFERENCES `client` (`ID_CLIENT`) ON DELETE RESTRICT ON UPDATE RESTRICT
) ENGINE = InnoDB AUTO_INCREMENT = 54 CHARACTER SET = utf8mb4 COLLATE = utf8mb4_general_ci ROW_FORMAT = Dynamic;

-- ----------------------------
-- Records of lieu
-- ----------------------------
INSERT INTO `lieu` VALUES (1, 'Salle B12#Salle de réunion', 1, 'GHYSKJDLDSMAZP-9');
INSERT INTO `lieu` VALUES (2, 'Salle Tetra#Salle de réunion ', 2, '0452281ACA4881');
INSERT INTO `lieu` VALUES (3, 'Salle Coca Cola#Bureau Open space', 3, '456978555');
INSERT INTO `lieu` VALUES (4, 'Salle Ampère#Salle de réunion', 4, 'AMP44');
INSERT INTO `lieu` VALUES (5, 'Tesla#Bureau Direction', 4, 'GHYSKJDLDSMAZP-0');
INSERT INTO `lieu` VALUES (6, 'Orangina Rouge#Bureau Open Space', 3, 'GHYSKJDLDSMAZP-8');
INSERT INTO `lieu` VALUES (7, 'Mobilis Room#Bureau Open Space', 2, 'GHYSKJDLDSMAZP-10');
INSERT INTO `lieu` VALUES (9, 'Pepsi#Salle de réunion', 3, '4747DFC3');
INSERT INTO `lieu` VALUES (10, 'wc domaine nc #Toilettes ', 2, '5357512E017240');
INSERT INTO `lieu` VALUES (11, 'refectoire#salle a manger', 1, '53FE5000013B40');
INSERT INTO `lieu` VALUES (12, 'usine poste controle#Usine', 3, '53AF5120014440');
INSERT INTO `lieu` VALUES (13, 'dock doniambo#Usine', 4, '533F102801DB40');
INSERT INTO `lieu` VALUES (14, 'Toilettes seches ext 0002#Toilettes', 1, '6BE54BA6');
INSERT INTO `lieu` VALUES (16, 'Chambre 120#Bâtiment A', 5, 'CHSCH110');
INSERT INTO `lieu` VALUES (17, 'Chambre 111#Bâtiment A', 5, 'CHCCH111');
INSERT INTO `lieu` VALUES (18, 'Chambre 112#Bâtiment A', 5, 'CHSCH11');
INSERT INTO `lieu` VALUES (19, 'Chambre 113#Bâtiment A', 5, 'CHSCH113');
INSERT INTO `lieu` VALUES (20, 'Chambre 114#Bâtiment A', 5, 'CHSCG114');
INSERT INTO `lieu` VALUES (21, 'Chambre 115#Bâtiment A', 5, 'CHSCH115');
INSERT INTO `lieu` VALUES (22, 'Consultation pédopsychiatrie#Bâtiment B', 5, 'CHSSAC001');
INSERT INTO `lieu` VALUES (23, 'Salle de pause infirmières #Bâtiment B', 5, 'CHSSAPIN001');
INSERT INTO `lieu` VALUES (24, 'Bibliothèque résident#Bâtiment C', 5, 'CHSBIB00');
INSERT INTO `lieu` VALUES (25, 'Réfectoire#Bâtiment C', 5, 'CHSREF');
INSERT INTO `lieu` VALUES (26, 'Cuisine#Bâtiment C', 5, 'CHSCUIS');
INSERT INTO `lieu` VALUES (27, 'Chambre 116#Bâtiment A', 5, 'chsch109');
INSERT INTO `lieu` VALUES (28, 'Chambre 119#Bâtiment A', 5, 'CHSCH119');
INSERT INTO `lieu` VALUES (29, 'Chambre 117#Bâtiment A', 5, 'CHSCH117');
INSERT INTO `lieu` VALUES (30, 'Chambre 118#Bâtiment A', 5, 'CHSCH118');
INSERT INTO `lieu` VALUES (31, 'Chambre 119#Bâtiment A', 5, 'CHS');
INSERT INTO `lieu` VALUES (32, 'Local poubelles#Bâtiment C', 5, 'CHSLCPBBB');
INSERT INTO `lieu` VALUES (33, 'Sanitaires hommes#Bâtiment C', 5, 'SANPBH');
INSERT INTO `lieu` VALUES (34, 'Sanitaires femmes#Bâtiment C', 5, 'SANPBF');
INSERT INTO `lieu` VALUES (35, 'Consultation psychiatrie générale#Bâtiment B', 5, 'CPCHS');
INSERT INTO `lieu` VALUES (36, 'Consultation gériatrique#Bâtiment B', 5, 'GERCHS');
INSERT INTO `lieu` VALUES (37, 'Accueil #Bâtiment B', 5, 'ACCURCHS');
INSERT INTO `lieu` VALUES (38, 'Salle d\'attente#Bâtiment B', 5, 'SALATCH');
INSERT INTO `lieu` VALUES (39, 'Consultation Alzheimer#Bâtiment B', 5, 'ALZCHS');
INSERT INTO `lieu` VALUES (40, 'Pharmacie#Bâtiment B', 5, 'PHCHS');
INSERT INTO `lieu` VALUES (41, 'Salle TV#Bâtiment C', 5, 'TVCHS');
INSERT INTO `lieu` VALUES (42, 'Salle de jeux#Bâtiment C', 5, 'SAJCHS');
INSERT INTO `lieu` VALUES (43, 'Salle consultation résident#Bâtiment C', 5, 'SACRCHS');
INSERT INTO `lieu` VALUES (44, 'Chambre isolement 01#Bâtiment C', 5, 'CHISCHS1');
INSERT INTO `lieu` VALUES (45, 'Chambre isolement 02#Bâtiment C', 5, 'CHSIS02');
INSERT INTO `lieu` VALUES (46, 'Chambre 110#Bâtiment A', 5, 'CHSCH110');
INSERT INTO `lieu` VALUES (47, 'Escalier B#Bâtiment A', 5, 'ESCHSB');
INSERT INTO `lieu` VALUES (48, 'Escalier C#Bâtiment B', 5, 'ESCHSB');
INSERT INTO `lieu` VALUES (49, 'Escalier A#Bâtiment C', 5, 'ESCHSA');
INSERT INTO `lieu` VALUES (50, '102MINIBAR', 6, '43534534');
INSERT INTO `lieu` VALUES (51, 'CHAMBRE 101#house keeping', 6, 'CH101HK');
INSERT INTO `lieu` VALUES (52, 'CHAMBRE 101#fridge', 6, 'CH101FRIDGE');
INSERT INTO `lieu` VALUES (53, 'Chambre 102 minibar', 1, '856955');

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
) ENGINE = InnoDB AUTO_INCREMENT = 73 CHARACTER SET = utf8mb4 COLLATE = utf8mb4_general_ci ROW_FORMAT = Dynamic;

-- ----------------------------
-- Records of passage
-- ----------------------------
INSERT INTO `passage` VALUES (1, 22, 5, '2022-10-07 10:00:00.000000', '2022-10-07 11:31:00.000000', NULL, NULL);
INSERT INTO `passage` VALUES (2, 41, 1, '2022-06-14 14:13:57.167000', '2022-06-14 15:12:02.700000', NULL, NULL);
INSERT INTO `passage` VALUES (3, 40, 2, '2022-08-13 15:10:03.820000', '2022-08-13 16:11:14.100000', NULL, NULL);
INSERT INTO `passage` VALUES (4, 49, 1, '2022-07-08 15:10:28.810000', '2022-07-08 16:11:38.800000', NULL, NULL);
INSERT INTO `passage` VALUES (5, 29, 1, '2022-07-13 15:35:29.980000', '2022-07-13 16:37:21.400000', NULL, NULL);
INSERT INTO `passage` VALUES (6, 47, 1, '2022-06-09 15:38:15.623000', '2022-06-09 16:38:40.187000', NULL, NULL);
INSERT INTO `passage` VALUES (7, 31, 1, '2022-07-02 15:38:52.483000', '2022-07-02 17:41:10.600000', NULL, NULL);
INSERT INTO `passage` VALUES (8, 18, 1, '2022-06-08 16:27:11.280000', '2022-06-08 17:28:23.977000', NULL, NULL);
INSERT INTO `passage` VALUES (9, 42, 1, '2022-09-09 20:29:06.657000', '2022-09-09 21:30:28.000000', NULL, NULL);
INSERT INTO `passage` VALUES (10, 37, 1, '2022-08-11 15:06:45.340000', '2022-07-11 16:06:55.617000', NULL, NULL);
INSERT INTO `passage` VALUES (11, 41, 2, '2022-09-09 21:07:06.150000', '2022-09-09 22:08:18.200000', NULL, NULL);
INSERT INTO `passage` VALUES (12, 1, 1, '2022-06-13 21:07:24.237000', '2022-06-13 22:08:38.600000', NULL, NULL);
INSERT INTO `passage` VALUES (13, 34, 1, '2022-10-27 15:07:45.360000', '2022-10-27 17:07:49.167000', NULL, NULL);
INSERT INTO `passage` VALUES (14, 16, 1, '2022-10-13 12:26:59.043000', '2022-10-10 12:27:05.197000', NULL, NULL);
INSERT INTO `passage` VALUES (15, 44, 1, '2022-07-13 14:27:11.760000', '2022-07-13 15:33:33.760000', NULL, NULL);
INSERT INTO `passage` VALUES (16, 26, 2, '2022-11-15 16:33:56.303000', '2022-10-15 17:34:00.277000', NULL, NULL);
INSERT INTO `passage` VALUES (17, 2, 2, '2022-03-19 18:42:35.520000', '2022-03-19 19:42:44.350000', NULL, NULL);
INSERT INTO `passage` VALUES (18, 26, 1, '2022-06-14 12:25:51.817000', '2022-05-06 13:27:08.200000', NULL, NULL);
INSERT INTO `passage` VALUES (19, 2, 1, '2022-02-14 15:55:07.280000', '2022-02-14 16:55:18.390000', NULL, NULL);
INSERT INTO `passage` VALUES (20, 22, 2, '2022-03-14 17:24:33.163000', '2022-03-14 18:25:18.437000', NULL, NULL);
INSERT INTO `passage` VALUES (21, 23, 1, '2022-10-14 17:37:49.997000', '2022-10-14 18:38:15.457000', NULL, NULL);
INSERT INTO `passage` VALUES (22, 2, 1, '2022-04-14 17:39:15.840000', '2022-04-14 18:39:25.710000', NULL, NULL);
INSERT INTO `passage` VALUES (23, 48, 1, '2022-01-20 13:39:37.363000', '2022-01-20 14:40:04.863000', NULL, NULL);
INSERT INTO `passage` VALUES (24, 22, 2, '2022-01-14 17:41:17.897000', '2022-01-14 18:41:34.300000', NULL, NULL);
INSERT INTO `passage` VALUES (25, 33, 1, '2022-10-15 10:22:05.770000', '2022-10-15 11:22:37.593000', NULL, NULL);
INSERT INTO `passage` VALUES (26, 25, 2, '2022-11-15 10:50:40.880000', '2022-11-15 11:50:41.603000', NULL, NULL);
INSERT INTO `passage` VALUES (27, 20, 2, '2022-05-15 10:50:43.327000', '2022-05-15 11:51:07.073000', NULL, NULL);
INSERT INTO `passage` VALUES (28, 27, 2, '2022-05-15 10:52:11.187000', '2022-05-15 11:52:30.787000', NULL, NULL);
INSERT INTO `passage` VALUES (29, 24, 1, '2022-06-11 11:18:04.283000', '2022-06-11 12:19:37.837000', NULL, NULL);
INSERT INTO `passage` VALUES (30, 40, 1, '2022-06-15 11:23:20.610000', '2022-06-15 12:23:29.930000', NULL, NULL);
INSERT INTO `passage` VALUES (31, 45, 1, '2022-11-15 11:24:06.027000', '2022-11-15 12:24:30.860000', NULL, NULL);
INSERT INTO `passage` VALUES (32, 11, 1, '2022-04-15 11:27:13.070000', '2022-04-15 12:27:23.057000', NULL, NULL);
INSERT INTO `passage` VALUES (33, 30, 1, '2022-07-15 11:27:48.787000', '2022-07-15 12:27:51.457000', NULL, NULL);
INSERT INTO `passage` VALUES (34, 22, 1, '2022-11-15 11:42:47.210000', '2022-11-15 12:42:47.637000', NULL, NULL);
INSERT INTO `passage` VALUES (35, 22, 1, '2022-11-15 11:47:09.330000', '2022-11-15 12:47:40.430000', NULL, NULL);
INSERT INTO `passage` VALUES (36, 43, 1, '2022-05-06 11:48:55.037000', '2022-05-06 13:49:30.687000', NULL, NULL);
INSERT INTO `passage` VALUES (37, 46, 1, '2022-06-15 11:53:20.067000', '2022-06-15 12:53:23.220000', NULL, NULL);
INSERT INTO `passage` VALUES (38, 24, 1, '2022-12-15 13:14:01.037000', '2022-12-15 14:14:24.497000', NULL, NULL);
INSERT INTO `passage` VALUES (39, 5, 1, '2022-02-15 13:15:27.977000', '2022-02-15 14:15:52.893000', NULL, NULL);
INSERT INTO `passage` VALUES (40, 32, 1, '2022-02-15 13:16:13.013000', '2022-02-15 14:16:24.797000', NULL, NULL);
INSERT INTO `passage` VALUES (41, 10, 1, '2022-02-03 10:17:00.113000', '2022-02-03 11:17:12.100000', NULL, NULL);
INSERT INTO `passage` VALUES (42, 36, 1, '2022-08-15 13:22:04.873000', '2022-08-15 14:22:14.777000', NULL, NULL);
INSERT INTO `passage` VALUES (43, 37, 1, '2022-01-15 13:22:48.447000', '2022-01-15 14:24:09.720000', NULL, NULL);
INSERT INTO `passage` VALUES (44, 25, 1, '2022-11-15 13:24:30.247000', '2022-11-15 14:53:20.083000', NULL, NULL);
INSERT INTO `passage` VALUES (45, 1, 1, '2022-05-15 13:24:30.247000', '2022-05-15 14:53:20.130000', NULL, NULL);
INSERT INTO `passage` VALUES (46, 44, 1, '2022-11-15 14:56:01.363000', '2022-11-15 15:56:01.490000', NULL, NULL);
INSERT INTO `passage` VALUES (47, 14, 1, '2022-04-16 06:36:21.757000', '2022-04-16 07:36:41.503000', NULL, NULL);
INSERT INTO `passage` VALUES (48, 18, 1, '2022-11-16 21:37:27.620000', '2022-11-16 22:37:31.353000', NULL, NULL);
INSERT INTO `passage` VALUES (49, 14, 1, '2022-05-08 11:38:42.810000', '2022-05-08 12:39:22.367000', NULL, NULL);
INSERT INTO `passage` VALUES (50, 14, 1, '2022-03-16 13:39:46.720000', '2022-03-16 14:40:03.623000', NULL, NULL);
INSERT INTO `passage` VALUES (51, 23, 1, '2022-03-16 21:41:10.770000', '2022-03-16 22:41:10.817000', NULL, NULL);
INSERT INTO `passage` VALUES (52, 14, 1, '2022-04-16 13:43:29.147000', '2022-04-16 14:43:38.770000', NULL, NULL);
INSERT INTO `passage` VALUES (53, 20, 1, '2023-01-04 10:48:08.217000', '2023-01-04 11:48:21.343000', NULL, NULL);
INSERT INTO `passage` VALUES (54, 43, 1, '2022-12-12 13:48:29.097000', '2022-12-12 14:48:34.283000', NULL, NULL);
INSERT INTO `passage` VALUES (55, 22, 2, '2022-12-15 16:49:06.737000', '2022-12-15 17:49:12.687000', NULL, NULL);
INSERT INTO `passage` VALUES (56, 21, 5, '2022-12-16 09:29:16.367000', '2022-12-16 10:29:17.050000', NULL, NULL);
INSERT INTO `passage` VALUES (57, 35, 2, '2022-09-08 20:29:23.763000', '2022-09-08 21:31:11.700000', NULL, NULL);
INSERT INTO `passage` VALUES (58, 39, 2, '2022-10-05 19:30:28.637000', '2022-10-05 20:30:29.447000', NULL, NULL);
INSERT INTO `passage` VALUES (59, 38, 2, '2022-09-22 20:30:30.797000', '2022-09-22 21:33:28.800000', NULL, NULL);
INSERT INTO `passage` VALUES (60, 24, 2, '2022-08-11 12:32:39.800000', '2022-08-11 13:36:45.600000', NULL, NULL);
INSERT INTO `passage` VALUES (61, 19, 5, '2022-09-13 11:06:10.400000', '2022-09-13 13:08:25.000000', NULL, NULL);
INSERT INTO `passage` VALUES (62, 18, 5, '2022-09-09 10:05:30.063000', '2022-09-09 12:07:57.000000', NULL, NULL);
INSERT INTO `passage` VALUES (63, 21, 2, '2022-09-14 15:15:25.920000', '2022-09-14 18:25:58.700000', NULL, NULL);
INSERT INTO `passage` VALUES (64, 17, 2, '2023-01-04 14:48:00.000000', '2023-01-04 15:48:00.000000', NULL, NULL);
INSERT INTO `passage` VALUES (65, 16, 2, '2023-01-10 07:53:00.000000', '2023-01-10 09:49:00.000000', NULL, NULL);
INSERT INTO `passage` VALUES (66, 1, 1, '2022-01-04 13:25:00.000000', '2022-01-04 14:26:00.000000', NULL, NULL);
INSERT INTO `passage` VALUES (67, 22, 5, '2022-01-04 10:17:00.000000', '2022-01-04 11:28:00.000000', NULL, NULL);
INSERT INTO `passage` VALUES (68, 28, 1, '2022-01-05 07:27:00.000000', '2022-01-05 08:29:00.000000', NULL, NULL);
INSERT INTO `passage` VALUES (69, 22, 1, '2023-01-17 17:21:00.000000', '2023-01-17 17:32:00.000000', NULL, NULL);
INSERT INTO `passage` VALUES (70, 40, 1, '2023-01-22 12:43:00.000000', '2023-01-22 15:36:00.000000', NULL, NULL);
INSERT INTO `passage` VALUES (71, 16, 2, '2023-01-23 07:45:00.000000', '2023-01-23 08:27:00.000000', NULL, NULL);
INSERT INTO `passage` VALUES (72, 51, 1, '2023-01-21 08:04:00.000000', '2023-02-02 09:50:00.000000', NULL, NULL);

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
  CONSTRAINT `FK_PASSAGE_TACHE_PASSAGE` FOREIGN KEY (`ID_PASSAGE`) REFERENCES `passage` (`ID_PASSAGE`) ON DELETE RESTRICT ON UPDATE RESTRICT,
  CONSTRAINT `FK_PASSAGE_TACHE_TACHE_LIEU` FOREIGN KEY (`ID_TL`) REFERENCES `tache_lieu` (`ID_TL`) ON DELETE RESTRICT ON UPDATE RESTRICT
) ENGINE = InnoDB AUTO_INCREMENT = 263 CHARACTER SET = utf8mb4 COLLATE = utf8mb4_general_ci ROW_FORMAT = Dynamic;

-- ----------------------------
-- Records of passage_tache
-- ----------------------------
INSERT INTO `passage_tache` VALUES (224, 57, 32, 0);
INSERT INTO `passage_tache` VALUES (225, 58, 14, 0);
INSERT INTO `passage_tache` VALUES (226, 58, 17, 0);
INSERT INTO `passage_tache` VALUES (227, 58, 21, 0);
INSERT INTO `passage_tache` VALUES (228, 58, 27, 0);
INSERT INTO `passage_tache` VALUES (229, 58, 28, 0);
INSERT INTO `passage_tache` VALUES (230, 58, 29, 0);
INSERT INTO `passage_tache` VALUES (231, 58, 30, 0);
INSERT INTO `passage_tache` VALUES (232, 59, 14, 0);
INSERT INTO `passage_tache` VALUES (233, 59, 17, 1);
INSERT INTO `passage_tache` VALUES (234, 59, 21, 0);
INSERT INTO `passage_tache` VALUES (235, 59, 27, 0);
INSERT INTO `passage_tache` VALUES (236, 59, 28, 0);
INSERT INTO `passage_tache` VALUES (237, 59, 29, 0);
INSERT INTO `passage_tache` VALUES (238, 59, 30, 0);
INSERT INTO `passage_tache` VALUES (239, 60, 15, 0);
INSERT INTO `passage_tache` VALUES (240, 60, 19, 0);
INSERT INTO `passage_tache` VALUES (241, 60, 22, 0);
INSERT INTO `passage_tache` VALUES (242, 60, 31, 0);
INSERT INTO `passage_tache` VALUES (243, 60, 32, 0);
INSERT INTO `passage_tache` VALUES (244, 61, 14, 0);
INSERT INTO `passage_tache` VALUES (245, 61, 17, 0);
INSERT INTO `passage_tache` VALUES (246, 61, 21, 0);
INSERT INTO `passage_tache` VALUES (247, 61, 27, 0);
INSERT INTO `passage_tache` VALUES (248, 61, 28, 1);
INSERT INTO `passage_tache` VALUES (249, 61, 29, 1);
INSERT INTO `passage_tache` VALUES (250, 61, 30, 0);
INSERT INTO `passage_tache` VALUES (251, 62, 14, 0);
INSERT INTO `passage_tache` VALUES (252, 62, 17, 1);
INSERT INTO `passage_tache` VALUES (253, 62, 21, 0);
INSERT INTO `passage_tache` VALUES (254, 62, 27, 1);
INSERT INTO `passage_tache` VALUES (255, 62, 28, 0);
INSERT INTO `passage_tache` VALUES (256, 62, 29, 0);
INSERT INTO `passage_tache` VALUES (257, 62, 30, 0);
INSERT INTO `passage_tache` VALUES (258, 63, 15, 0);
INSERT INTO `passage_tache` VALUES (259, 63, 19, 0);
INSERT INTO `passage_tache` VALUES (260, 63, 22, 1);
INSERT INTO `passage_tache` VALUES (261, 63, 31, 1);
INSERT INTO `passage_tache` VALUES (262, 63, 32, 0);

-- ----------------------------
-- Table structure for tache
-- ----------------------------
DROP TABLE IF EXISTS `tache`;
CREATE TABLE `tache`  (
  `ID_TACHE` int(11) NOT NULL AUTO_INCREMENT,
  `NOM` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NOT NULL,
  `DESCRIPTION` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NULL,
  PRIMARY KEY (`ID_TACHE`) USING BTREE
) ENGINE = InnoDB AUTO_INCREMENT = 19 CHARACTER SET = utf8mb4 COLLATE = utf8mb4_general_ci ROW_FORMAT = Dynamic;

-- ----------------------------
-- Records of tache
-- ----------------------------
INSERT INTO `tache` VALUES (1, 'Dépoussiérage des sols', 'processus de désinfection renforcé indispensable.');
INSERT INTO `tache` VALUES (2, 'Décapage et nettoyage des sols', 'Attention aux tâches tenaces');
INSERT INTO `tache` VALUES (3, 'Nettoyage des vitres', 'Nettoyage vitrerie intérieure/extérieure avec enlèvement des traces de doigts');
INSERT INTO `tache` VALUES (4, 'Nettoyage de la VMC', NULL);
INSERT INTO `tache` VALUES (7, 'Piquetage du parking', NULL);
INSERT INTO `tache` VALUES (8, 'Décapage des sols carrelés', NULL);
INSERT INTO `tache` VALUES (9, 'Désinfections des poubelles', NULL);
INSERT INTO `tache` VALUES (10, 'Effaçage des tags', NULL);
INSERT INTO `tache` VALUES (11, 'Lustrage de l\'ascenseur ', 'Lustrage de l\'ascenseur avec le produit spécial inox');
INSERT INTO `tache` VALUES (12, 'Décapage et nettoyage des plafonds', NULL);
INSERT INTO `tache` VALUES (13, 'Nettoyage garde-corps et rampes', 'Mise à blanc ');
INSERT INTO `tache` VALUES (14, 'Décapage et nettoyage de terrasse', 'Utiliser le produit B673');
INSERT INTO `tache` VALUES (15, 'Décapage et nettoyage des murs', NULL);
INSERT INTO `tache` VALUES (16, 'HEINEKEN$15', NULL);
INSERT INTO `tache` VALUES (17, 'VODKA$20', NULL);
INSERT INTO `tache` VALUES (18, 'lustrer la tv', 'Lustrer avec chiffon sec (pas de produit)');

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
  CONSTRAINT `FK_TACHE_LIEU_LIEU` FOREIGN KEY (`ID_LIEU`) REFERENCES `lieu` (`ID_LIEU`) ON DELETE RESTRICT ON UPDATE RESTRICT,
  CONSTRAINT `FK_TACHE_LIEU_TACHE` FOREIGN KEY (`ID_TACHE`) REFERENCES `tache` (`ID_TACHE`) ON DELETE RESTRICT ON UPDATE RESTRICT
) ENGINE = InnoDB AUTO_INCREMENT = 176 CHARACTER SET = utf8mb4 COLLATE = utf8mb4_general_ci ROW_FORMAT = Dynamic;

-- ----------------------------
-- Records of tache_lieu
-- ----------------------------
INSERT INTO `tache_lieu` VALUES (1, 1, 1);
INSERT INTO `tache_lieu` VALUES (2, 2, 1);
INSERT INTO `tache_lieu` VALUES (3, 3, 1);
INSERT INTO `tache_lieu` VALUES (4, 4, 1);
INSERT INTO `tache_lieu` VALUES (24, 7, 1);
INSERT INTO `tache_lieu` VALUES (25, 8, 1);
INSERT INTO `tache_lieu` VALUES (26, 9, 1);
INSERT INTO `tache_lieu` VALUES (173, 16, 1);
INSERT INTO `tache_lieu` VALUES (5, 1, 2);
INSERT INTO `tache_lieu` VALUES (6, 2, 2);
INSERT INTO `tache_lieu` VALUES (7, 3, 2);
INSERT INTO `tache_lieu` VALUES (8, 4, 2);
INSERT INTO `tache_lieu` VALUES (69, 1, 3);
INSERT INTO `tache_lieu` VALUES (68, 3, 3);
INSERT INTO `tache_lieu` VALUES (54, 9, 3);
INSERT INTO `tache_lieu` VALUES (61, 8, 4);
INSERT INTO `tache_lieu` VALUES (63, 3, 5);
INSERT INTO `tache_lieu` VALUES (62, 4, 5);
INSERT INTO `tache_lieu` VALUES (66, 1, 6);
INSERT INTO `tache_lieu` VALUES (65, 4, 6);
INSERT INTO `tache_lieu` VALUES (64, 9, 6);
INSERT INTO `tache_lieu` VALUES (67, 13, 7);
INSERT INTO `tache_lieu` VALUES (71, 1, 9);
INSERT INTO `tache_lieu` VALUES (70, 4, 9);
INSERT INTO `tache_lieu` VALUES (56, 8, 9);
INSERT INTO `tache_lieu` VALUES (9, 1, 10);
INSERT INTO `tache_lieu` VALUES (11, 2, 10);
INSERT INTO `tache_lieu` VALUES (12, 3, 10);
INSERT INTO `tache_lieu` VALUES (13, 4, 10);
INSERT INTO `tache_lieu` VALUES (37, 11, 10);
INSERT INTO `tache_lieu` VALUES (27, 1, 11);
INSERT INTO `tache_lieu` VALUES (28, 2, 11);
INSERT INTO `tache_lieu` VALUES (29, 3, 11);
INSERT INTO `tache_lieu` VALUES (30, 4, 11);
INSERT INTO `tache_lieu` VALUES (14, 7, 11);
INSERT INTO `tache_lieu` VALUES (17, 8, 11);
INSERT INTO `tache_lieu` VALUES (21, 9, 11);
INSERT INTO `tache_lieu` VALUES (60, 14, 11);
INSERT INTO `tache_lieu` VALUES (172, 18, 11);
INSERT INTO `tache_lieu` VALUES (31, 3, 12);
INSERT INTO `tache_lieu` VALUES (32, 4, 12);
INSERT INTO `tache_lieu` VALUES (19, 8, 12);
INSERT INTO `tache_lieu` VALUES (22, 9, 12);
INSERT INTO `tache_lieu` VALUES (15, 11, 12);
INSERT INTO `tache_lieu` VALUES (16, 7, 13);
INSERT INTO `tache_lieu` VALUES (20, 8, 13);
INSERT INTO `tache_lieu` VALUES (23, 9, 13);
INSERT INTO `tache_lieu` VALUES (52, 7, 14);
INSERT INTO `tache_lieu` VALUES (57, 8, 14);
INSERT INTO `tache_lieu` VALUES (51, 9, 14);
INSERT INTO `tache_lieu` VALUES (53, 12, 14);
INSERT INTO `tache_lieu` VALUES (72, 2, 16);
INSERT INTO `tache_lieu` VALUES (73, 3, 16);
INSERT INTO `tache_lieu` VALUES (74, 9, 16);
INSERT INTO `tache_lieu` VALUES (75, 2, 17);
INSERT INTO `tache_lieu` VALUES (76, 3, 17);
INSERT INTO `tache_lieu` VALUES (77, 9, 17);
INSERT INTO `tache_lieu` VALUES (78, 2, 18);
INSERT INTO `tache_lieu` VALUES (80, 3, 18);
INSERT INTO `tache_lieu` VALUES (79, 9, 18);
INSERT INTO `tache_lieu` VALUES (83, 2, 19);
INSERT INTO `tache_lieu` VALUES (81, 3, 19);
INSERT INTO `tache_lieu` VALUES (82, 9, 19);
INSERT INTO `tache_lieu` VALUES (84, 2, 20);
INSERT INTO `tache_lieu` VALUES (86, 3, 20);
INSERT INTO `tache_lieu` VALUES (85, 9, 20);
INSERT INTO `tache_lieu` VALUES (88, 2, 21);
INSERT INTO `tache_lieu` VALUES (87, 3, 21);
INSERT INTO `tache_lieu` VALUES (89, 9, 21);
INSERT INTO `tache_lieu` VALUES (90, 1, 22);
INSERT INTO `tache_lieu` VALUES (91, 9, 22);
INSERT INTO `tache_lieu` VALUES (92, 12, 22);
INSERT INTO `tache_lieu` VALUES (93, 15, 22);
INSERT INTO `tache_lieu` VALUES (95, 1, 23);
INSERT INTO `tache_lieu` VALUES (94, 4, 23);
INSERT INTO `tache_lieu` VALUES (96, 9, 23);
INSERT INTO `tache_lieu` VALUES (98, 1, 24);
INSERT INTO `tache_lieu` VALUES (99, 3, 24);
INSERT INTO `tache_lieu` VALUES (97, 4, 24);
INSERT INTO `tache_lieu` VALUES (101, 9, 24);
INSERT INTO `tache_lieu` VALUES (100, 13, 24);
INSERT INTO `tache_lieu` VALUES (102, 1, 25);
INSERT INTO `tache_lieu` VALUES (104, 2, 25);
INSERT INTO `tache_lieu` VALUES (103, 3, 25);
INSERT INTO `tache_lieu` VALUES (106, 2, 26);
INSERT INTO `tache_lieu` VALUES (107, 9, 26);
INSERT INTO `tache_lieu` VALUES (105, 12, 26);
INSERT INTO `tache_lieu` VALUES (117, 1, 27);
INSERT INTO `tache_lieu` VALUES (118, 3, 27);
INSERT INTO `tache_lieu` VALUES (119, 8, 27);
INSERT INTO `tache_lieu` VALUES (120, 1, 28);
INSERT INTO `tache_lieu` VALUES (122, 2, 28);
INSERT INTO `tache_lieu` VALUES (121, 3, 28);
INSERT INTO `tache_lieu` VALUES (123, 1, 29);
INSERT INTO `tache_lieu` VALUES (124, 2, 29);
INSERT INTO `tache_lieu` VALUES (125, 3, 29);
INSERT INTO `tache_lieu` VALUES (126, 1, 30);
INSERT INTO `tache_lieu` VALUES (127, 2, 30);
INSERT INTO `tache_lieu` VALUES (128, 3, 30);
INSERT INTO `tache_lieu` VALUES (129, 1, 31);
INSERT INTO `tache_lieu` VALUES (130, 2, 31);
INSERT INTO `tache_lieu` VALUES (131, 3, 31);
INSERT INTO `tache_lieu` VALUES (114, 2, 32);
INSERT INTO `tache_lieu` VALUES (115, 9, 32);
INSERT INTO `tache_lieu` VALUES (116, 8, 33);
INSERT INTO `tache_lieu` VALUES (134, 9, 33);
INSERT INTO `tache_lieu` VALUES (132, 8, 34);
INSERT INTO `tache_lieu` VALUES (133, 9, 34);
INSERT INTO `tache_lieu` VALUES (136, 2, 35);
INSERT INTO `tache_lieu` VALUES (135, 4, 35);
INSERT INTO `tache_lieu` VALUES (137, 9, 35);
INSERT INTO `tache_lieu` VALUES (138, 15, 35);
INSERT INTO `tache_lieu` VALUES (139, 1, 36);
INSERT INTO `tache_lieu` VALUES (140, 2, 36);
INSERT INTO `tache_lieu` VALUES (141, 9, 36);
INSERT INTO `tache_lieu` VALUES (142, 1, 37);
INSERT INTO `tache_lieu` VALUES (143, 9, 37);
INSERT INTO `tache_lieu` VALUES (144, 10, 37);
INSERT INTO `tache_lieu` VALUES (145, 1, 38);
INSERT INTO `tache_lieu` VALUES (146, 10, 38);
INSERT INTO `tache_lieu` VALUES (147, 15, 38);
INSERT INTO `tache_lieu` VALUES (148, 1, 39);
INSERT INTO `tache_lieu` VALUES (150, 9, 39);
INSERT INTO `tache_lieu` VALUES (149, 1, 40);
INSERT INTO `tache_lieu` VALUES (151, 1, 41);
INSERT INTO `tache_lieu` VALUES (152, 2, 41);
INSERT INTO `tache_lieu` VALUES (153, 4, 41);
INSERT INTO `tache_lieu` VALUES (155, 1, 42);
INSERT INTO `tache_lieu` VALUES (154, 2, 42);
INSERT INTO `tache_lieu` VALUES (156, 9, 42);
INSERT INTO `tache_lieu` VALUES (112, 8, 43);
INSERT INTO `tache_lieu` VALUES (113, 15, 43);
INSERT INTO `tache_lieu` VALUES (108, 1, 44);
INSERT INTO `tache_lieu` VALUES (109, 2, 44);
INSERT INTO `tache_lieu` VALUES (110, 1, 45);
INSERT INTO `tache_lieu` VALUES (111, 2, 45);
INSERT INTO `tache_lieu` VALUES (158, 1, 46);
INSERT INTO `tache_lieu` VALUES (159, 3, 46);
INSERT INTO `tache_lieu` VALUES (157, 8, 46);
INSERT INTO `tache_lieu` VALUES (160, 1, 47);
INSERT INTO `tache_lieu` VALUES (161, 2, 47);
INSERT INTO `tache_lieu` VALUES (162, 13, 47);
INSERT INTO `tache_lieu` VALUES (163, 1, 48);
INSERT INTO `tache_lieu` VALUES (164, 2, 48);
INSERT INTO `tache_lieu` VALUES (165, 13, 48);
INSERT INTO `tache_lieu` VALUES (166, 1, 49);
INSERT INTO `tache_lieu` VALUES (167, 2, 49);
INSERT INTO `tache_lieu` VALUES (168, 13, 49);
INSERT INTO `tache_lieu` VALUES (169, 16, 52);
INSERT INTO `tache_lieu` VALUES (170, 17, 52);
INSERT INTO `tache_lieu` VALUES (175, 16, 53);
INSERT INTO `tache_lieu` VALUES (174, 17, 53);

SET FOREIGN_KEY_CHECKS = 1;
