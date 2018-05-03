﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Stratego
{
   public class GrilleJeu
   {
      #region Static

      /// <summary>
      /// La taille de la grille de jeu. Assume une grille de jeu carrée (X par X).
      /// </summary>
      public const int TAILLE_GRILLE_JEU = 10;

      #endregion
      private List<List<CaseJeu>> GrilleCases { get; set; }

      private Couleur CouleurJoueur { get; set; }

      public GrilleJeu(Couleur couleurJoueur)
      {
         InitialiserGrille();
         CouleurJoueur = couleurJoueur;
      }

      private void InitialiserGrille()
      {
         List<CaseJeu> colonne;
         GrilleCases = new List<List<CaseJeu>>();

         // Créer les cases et les structurer dans une grille à deux dimensions.
         for (int i = 0; i < TAILLE_GRILLE_JEU; i++)
         {
            colonne = new List<CaseJeu>();

            for (int j = 0; j < TAILLE_GRILLE_JEU; j++)
            {
               // Coordonnées des lacs : I (2, 3, 6, 7) - J (4, 5)
               if ((i == 2 || i == 3 || i == 6 || i == 7) && (j == 4 || j == 5))
               {
                  colonne.Add(new CaseJeu("Lac"));
               }
               else
               {
                  colonne.Add(new CaseJeu("Terrain"));
               }
            }

            GrilleCases.Add(colonne);
         }

         // Créer les liens de voisinage entre les cases de la grille.
         LierCasesGrille();
      }

      private void LierCasesGrille()
      {
         for (int i = 0; i < TAILLE_GRILLE_JEU; i++)
         {
            for (int j = 0; j < TAILLE_GRILLE_JEU; j++)
            {
               // Les coins.
               if ((i == 0 || i == TAILLE_GRILLE_JEU - 1) && (j == 0 || j == TAILLE_GRILLE_JEU - 1))
               {
                  if (i == 0)
                  {
                     GrilleCases[i][j].VoisinDroite = GrilleCases[i + 1][j];
                  }
                  else
                  {
                     GrilleCases[i][j].VoisinGauche = GrilleCases[i - 1][j];
                  }

                  if (j == 0)
                  {
                     GrilleCases[i][j].VoisinArriere = GrilleCases[i][j + 1];
                  }
                  else 
                  {
                     GrilleCases[i][j].VoisinAvant = GrilleCases[i][j - 1];
                  }
               }
               // Côtés verticaux.
               else if (i == 0 || i == TAILLE_GRILLE_JEU - 1)
               {
                  if (i == 0)
                  {
                     GrilleCases[i][j].VoisinAvant = GrilleCases[i][j - 1];
                     GrilleCases[i][j].VoisinDroite = GrilleCases[i + 1][j];
                     GrilleCases[i][j].VoisinArriere = GrilleCases[i][j + 1];
                  }
                  else
                  {
                     GrilleCases[i][j].VoisinGauche = GrilleCases[i - 1][j];
                     GrilleCases[i][j].VoisinAvant = GrilleCases[i][j - 1];
                     GrilleCases[i][j].VoisinArriere = GrilleCases[i][j + 1];
                  }
               }
               // Côtés horizontaux.
               else if (j == 0 || j == TAILLE_GRILLE_JEU - 1)
               {
                  if (j == 0)
                  {
                     GrilleCases[i][j].VoisinGauche = GrilleCases[i - 1][j];
                     GrilleCases[i][j].VoisinDroite = GrilleCases[i + 1][j];
                     GrilleCases[i][j].VoisinArriere = GrilleCases[i][j + 1];
                  }
                  else
                  {
                     GrilleCases[i][j].VoisinGauche = GrilleCases[i - 1][j];
                     GrilleCases[i][j].VoisinAvant = GrilleCases[i][j - 1];
                     GrilleCases[i][j].VoisinDroite = GrilleCases[i + 1][j];
                  }
               }
               else 
               {
                  GrilleCases[i][j].VoisinGauche = GrilleCases[i - 1][j];
                  GrilleCases[i][j].VoisinAvant = GrilleCases[i][j - 1];
                  GrilleCases[i][j].VoisinDroite = GrilleCases[i + 1][j];
                  GrilleCases[i][j].VoisinArriere = GrilleCases[i][j + 1];
               }
            }
         }
      }

      public ReponseDeplacement ResoudreDeplacement(Coordonnee pointDepart, Coordonnee pointCible)
      {
         ReponseDeplacement reponse = new ReponseDeplacement();

         CaseJeu caseDepart, caseCible;

         if (EstCoordonneeValide(pointDepart) && EstCoordonneeValide(pointCible))
         {
            caseDepart = GrilleCases[pointDepart.X][pointDepart.Y];
            caseCible = GrilleCases[pointCible.X][pointCible.Y];

            if (caseDepart.EstOccupe() && EstDeplacementPermis(pointDepart, pointCible))
            {
               // Faire le déplacement.
               reponse.PiecesEliminees = caseCible.ResoudreAttaque(caseDepart.Occupant);
               caseDepart.Occupant = null;

               reponse.DeplacementFait = true;
            }
            else
            {
               reponse.DeplacementFait = false;
            }
         }
         else
         {
            reponse.DeplacementFait = false;
         }

         return reponse;
      }

      public bool EstDeplacementPermis(Coordonnee pointDepart, Coordonnee pointCible)
      {
         return ( EstCoordonneeValide(pointDepart) && EstCoordonneeValide(pointCible)
                && !EstCoordonneeLac(pointDepart) && !EstCoordonneeLac(pointCible)
                && GrilleCases[pointDepart.X][pointDepart.Y].EstDeplacementLegal(GrilleCases[pointCible.X][pointCible.Y])
                );
      }

      private bool EstCoordonneeValide(Coordonnee p)
      {
         if ((p.X >= 0 && p.X < TAILLE_GRILLE_JEU) && (p.Y >= 0 && p.Y < TAILLE_GRILLE_JEU))
         {
            return true;
         }
         else
         {
            return false;
         }
      }

      public bool EstCoordonneeLac(Coordonnee p)
      {
         // Coordonnées des lacs : I (2, 3, 6, 7) - J (4, 5)
         if ((p.X == 2 || p.X == 3 || p.X == 6 || p.X == 7) && (p.Y == 4 || p.Y == 5))
         {
            return true;
         }
         else
         {
            return false;
         }
      }

      public bool EstCaseOccupee(Coordonnee p)
      {
         return ((GrilleCases[(int)p.X][(int)p.Y]).EstOccupe());
      }

      public bool PositionnerPieces(List<Piece> lstPieces, Couleur couleurJoueur)
      {
         bool positionnementApplique = false;

         int compteur = 0;
         int decallage = 0;
         
         if (couleurJoueur == CouleurJoueur)
         {
            decallage = 6;
         }

         if (!PositionnementFait(couleurJoueur) && lstPieces.Count == 40)
         {
            positionnementApplique = true;

            for (int j = 0 + decallage; j < 4 + decallage; j++)
            {
               for (int i = 0; i < TAILLE_GRILLE_JEU; i++)
               {
                  GrilleCases[i][j].Occupant = lstPieces[compteur];

                  compteur++;
               }
            }
         }

         return positionnementApplique;
      }

      private bool PositionnementFait(Couleur couleurJoueur)
      {
         bool pieceTrouvee = false;

         for (int i = 0; i < TAILLE_GRILLE_JEU; i++)
         {
            for (int j = 0; j < TAILLE_GRILLE_JEU; j++)
            {
               if (GrilleCases[i][j].Occupant != null 
                     && ((GrilleCases[i][j].Occupant.EstDeCouleur(Couleur.Rouge) && couleurJoueur == Couleur.Rouge)
                        || (GrilleCases[i][j].Occupant.EstDeCouleur(Couleur.Bleu) && couleurJoueur == Couleur.Bleu)))
               {
                  pieceTrouvee = true;

                  // Inutile de chercher plus.
                  j = TAILLE_GRILLE_JEU;
                  i = TAILLE_GRILLE_JEU;

                  // ^ Les break, ça existe aussi ! ^
               }
            }
         }

         return pieceTrouvee;
      }

      public Piece ObtenirPiece(Coordonnee p)
      {
         return GrilleCases[p.X][p.Y].Occupant;
      }

      public Couleur ObtenirCouleurPiece(Coordonnee p)
      {
         return GrilleCases[p.X][p.Y].Occupant.Couleur;
      }

   }
}
