# 📘 StreamMaster Documentation

This document outlines the steps to set up and manage documentation for the [StreamMaster](https://github.com/SenexCrenshaw/StreamMaster) project using MkDocs. The setup includes support for internationalization (i18n) and the use of MkDocs Material theme for a modern look and feel.

## Why Contribute to the Documentation?

The documentation is the first thing users and developers refer to when using or contributing to StreamMaster. By helping to improve and maintain the documentation, you are ensuring that StreamMaster remains accessible and easy to use for the community.

Your contributions to the documentation:

- Help others learn and use StreamMaster more effectively.
- Improve clarity for non-native English speakers through better i18n support.
- Enable developers to more easily contribute to the project.

Even small updates like correcting typos or clarifying instructions make a big difference!

## 🛠 Prerequisites

To generate and serve documentation locally, you will need Python installed. Make sure `pip`, Python's package manager, is also available.

### Installing MkDocs and Plugins

To install MkDocs and the required plugins for i18n and theming, run the following command:

```bash
python -m pip install mkdocs mkdocs-i18n mkdocs-material mkdocs-static-i18n mkdocs-include-markdown-plugin
```

This installs the following:

- `mkdocs`: The static site generator.
- `mkdocs-i18n`: For handling internationalization.
- `mkdocs-material`: A popular theme with a modern design.
- `mkdocs-static-i18n`: Adds static internationalization support.

## Local Development

To build and serve the documentation locally during development, follow these steps.

### Building the Docs

To generate the static documentation:

```bash
mkdocs build
```

### Serving the Docs Locally

To run a development server that watches for changes and automatically reloads:

```bash
mkdocs serve
```

This will host the documentation locally on `http://localhost:8000`.

## Production Build

When you're ready to deploy the documentation to production, ensure you clean the previous build and set the site URL correctly. Run the following command:

```bash
mkdocs build --clean --site-url https://senexcrenshaw.github.io/StreamMaster/
```

This builds a clean version of the documentation and sets the correct URL for the production site.

## 📝 Contributing to the Documentation

Documentation files live under the `StreamMaster.Docs\streammaster` folder in the [StreamMaster repository](https://github.com/SenexCrenshaw/StreamMaster).

To contribute to the documentation:

- **Create a new branch** for your changes.
- **Ensure that English (`en`) is always updated**. English serves as the primary language, and all content must be updated in English.
- **Provide the best possible translations** for other supported languages, such as Spanish (`es`), French (`fr`), German (`de`), and any other supported languages. While these translations don't have to be perfect, they should aim to accurately reflect the English content.
  - English files live under `docs/en`.
  - Translations live under their respective directories (e.g., `docs/es` for Spanish, `docs/fr` for French, etc.).
- **Test** all changes by running `mkdocs serve` locally for both the English version and any updated translations.
- **Submit a Pull Request (PR)** for review.

### Getting Started in 3 Easy Steps!

1. Fork the repository and clone it to your local machine.
2. Create a new branch for your changes.
3. Ensure that English (`en`) is updated and provide the best possible translations for other supported languages, then submit a PR.

That’s it! 🎉 You’ve contributed to StreamMaster!

## Tips for Writing Good Documentation

- **Be Clear and Concise:** Focus on the main points, and avoid overly technical language where possible.
- **Use Examples:** Code snippets or visual aids help clarify complex concepts.
- **Be Consistent:** Keep tone and terminology consistent across all sections.
- **Test Everything:** Ensure that all code examples, commands, and instructions work as expected.

## Contributor Recognition 🌟

We appreciate every contribution, no matter how small! All contributors will be added to the StreamMaster Documentation Hall of Fame, featured below:

[View all contributors](Contributors.md)

## We Need Your Help! 🤝

StreamMaster is constantly growing, and we need the community's help to keep the documentation top-notch. No contribution is too small, whether it's fixing typos, adding examples, or translating content.

Jump in, and let’s make StreamMaster better together! ✨

## Need Help or Have Questions? Join Us on Discord! 🎮

For any questions, support, or discussions, you can join the official **StreamMaster Discord server**.

👉 [Join StreamMaster Discord](https://discord.gg/gFz7EtHhG2) 👈

We’re here to help, and you’ll find an active community of developers and users. Feel free to ask questions, report issues, or discuss new ideas for improving StreamMaster!
