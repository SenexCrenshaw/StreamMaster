# 📘 Documentation Streammaster

Ce document décrit les étapes pour configurer et gérer la documentation pour le projet [Streammaster] (https://github.com/senexcrenshaw/streammaster) à l'aide de mkdocs.La configuration comprend la prise en charge de l'internationalisation (I18N) et l'utilisation du thème matériel MKDOCS pour un look et une sensation modernes.

## Pourquoi contribuer à la documentation?

La documentation est la première chose que les utilisateurs et les développeurs se réfèrent lors de l'utilisation ou de la contribution à StreamMaster.En aidant à améliorer et à maintenir la documentation, vous vous assurez que StreamMaster reste accessible et facile à utiliser pour la communauté.

Vos contributions à la documentation:
- Aidez les autres à apprendre et à utiliser Streammaster plus efficacement.
- Améliorez la clarté des anglophones non natifs grâce à un meilleur soutien I18N.
- Permettez aux développeurs de contribuer plus facilement au projet.

Même les petites mises à jour comme la correction des fautes de frappe ou les instructions de clarification font une grande différence!

## 🛠 Prérequis

Pour générer et servir la documentation localement, vous aurez besoin de Python installé.Assurez-vous que «Pip», le gestionnaire de packages de Python, est également disponible.

### Installation des MKDOC et des plugins

Pour installer les MKDOC et les plugins requis pour i18n et theming, exécutez la commande suivante:

`` `bash
Python -M PIP Install Mkdocs MKDOCS-I18N MKDOCS-MATEIAL MKDOCS-STATIQUE-I18N
`` '

Cela installe ce qui suit:

- `mkdocs`: le générateur de sites statique.
- `Mkdocs-i18n`: pour gérer l'internationalisation.
- «Mkdocs-material»: un thème populaire avec un design moderne.
- `MkDocs-static-i18n`: ajoute un support d'internationalisation statique.

## Développement local

Pour construire et servir la documentation localement pendant le développement, suivez ces étapes.

### Construire les documents

Pour générer la documentation statique:

`` `bash
Mkdocs Build
`` '

### servant les documents localement

Pour exécuter un serveur de développement qui regarde les modifications et recharge automatiquement:

`` `bash
MKDOCS sert
`` '

Cela hébergera localement la documentation sur `http: // localhost: 8000`.

## build de production

Lorsque vous êtes prêt à déployer la documentation en production, assurez-vous de nettoyer la construction précédente et de définir correctement l'URL du site.Exécutez la commande suivante:

`` `bash
Mkdocs Build - Clean ---site-url https://senexcrenshaw.github.io/streammaster/
`` '

Cela construit une version propre de la documentation et définit l'URL correcte pour le site de production.

## 📝 Cont contribuant à la documentation

Les fichiers de documentation en direct dans le dossier `StreamMaster.Docs \ StreamMaster` dans le référentiel [StreamMaster] (https://github.com/senexcrenshaw/streammaster).

Pour contribuer à la documentation:

- ** Créez une nouvelle branche ** pour vos modifications.
- ** Assurez-vous que l'anglais (`EN`) est toujours mis à jour **.L'anglais sert de langue principale et tout le contenu doit être mis à jour en anglais.
- ** Fournir les meilleures traductions possibles ** pour d'autres langues prises en charge, comme l'espagnol (`` es '), le français (`` FR'), l'allemand (`` de ') et toutes les autres langues prises en charge.Bien que ces traductions ne soient pas parfaites, elles devraient viser à refléter avec précision le contenu anglais.
- Les fichiers anglais vivent sous `docs / en`.
- Les traductions vivent sous leurs répertoires respectifs (par exemple, «Docs / ES» pour l'espagnol, «Docs / FR» pour le français, etc.).
- ** Tester ** Tous les modifications en exécutant `MKDOCS servent localement pour la version anglaise et toutes les traductions mises à jour.
- ** Soumettre une demande de traction (pr) ** pour examen.

### En démarrage en 3 étapes faciles!

1. Fourk le référentiel et le cloner sur votre machine locale.
2. Créez une nouvelle branche pour vos modifications.
3. Assurez-vous que l'anglais (`` en`) est mis à jour et fournit les meilleures traductions possibles pour d'autres langues prises en charge, puis soumettez un PR.

C'est ça!🎉 Vous avez contribué à Streammaster!

## Conseils pour écrire une bonne documentation

- ** Soyez clair et concis: ** Concentrez-vous sur les principaux points et évitez le langage trop technique dans la mesure du possible.
- ** Utiliser des exemples: ** Les extraits de code ou les aides visuelles aident à clarifier les concepts complexes.
- ** Soyez cohérent: ** Gardez le ton et la terminologie cohérents dans toutes les sections.
- ** Testez tout: ** Assurez-vous que tous les exemples de code, commandes et instructions fonctionnent comme prévu.

## Reconnaissance des contributeurs 🌟

Nous apprécions chaque contribution, peu importe la petite taille!Tous les contributeurs seront ajoutés au Streammaster Documentation Hall of Fame, ci-dessous:

[Voir tous les contributeurs] (contributeurs.md)

## Nous avons besoin de votre aide!🤝

Streammaster se développe constamment et nous avons besoin de l'aide de la communauté pour maintenir la documentation de premier ordre.Aucune contribution n'est trop petite, qu'il s'agisse de réparer les fautes de frappe, d'ajouter des exemples ou de traduire du contenu.

Sautez et rendons Streammaster meilleur ensemble!✨

## Besoin d'aide ou avez des questions?Rejoignez-nous sur Discord!🎮

Pour toute question, support ou discussion, vous pouvez rejoindre le Streammaster Discord Server officiel **.

👉 [rejoindre StreamMaster Discord] (https://discord.gg/gfz7ethhg2) 👈

Nous sommes là pour aider et vous trouverez une communauté active de développeurs et d'utilisateurs.N'hésitez pas à poser des questions, à signaler des problèmes ou à discuter de nouvelles idées pour améliorer Streammaster!