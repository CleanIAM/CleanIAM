import { defineConfig } from "vitepress";

// https://vitepress.dev/reference/site-config
export default defineConfig({
  title: "CleanIAM",
  description: "Identity and Access Management System for .NET",
  base: "/CleanIAM/",
  head: [["link", { rel: "icon", href: "favicon.png" }]],
  themeConfig: {
    logo: "Figures/cleaniam-logo.svg",
    search: {
      provider: "local",
    },
    nav: [
      { text: "Home", link: "/" },
      { text: "Getting Started", link: "/introduction/getting-started" },
      { text: "Architecture", link: "/architecture/architecture" },
    ],

    sidebar: [
      {
        text: "Introduction",
        items: [
          { text: "Getting Started", link: "/introduction/getting-started" },
          { text: "Project Overview", link: "/introduction/project-overview" },
        ],
      },
      {
        text: "Architecture",
        items: [
          { text: "Overview", link: "/architecture/architecture" },
          { text: "Vertical Slices", link: "/architecture/slices" },
          {
            text: "Event-Driven Design",
            link: "/architecture/event-driven-design",
          },
        ],
      },
      {
        text: "Slices",
        items: [
          { text: "Identity", link: "/slices/identity" },
          { text: "Applications", link: "/slices/applications" },
          { text: "Users", link: "/slices/users" },
          { text: "Scopes", link: "/slices/scopes" },
          { text: "Tenants", link: "/slices/tenants" },
        ],
      },
      {
        text: "Frontend",
        items: [
          { text: "Overview", link: "/frontend/overview" },
          { text: "MVC Views", link: "/frontend/mvc" },
          { text: "React Management Portal", link: "/frontend/react" },
        ],
      },
      {
        text: "Implementation",
        items: [
          { text: "Overview", link: "/implementation/" },
          {
            text: "Project Structure",
            link: "/implementation/project-structure",
          },
          { text: "CQRS Pattern", link: "/implementation/cqrs-pattern" },
          { text: "Error Handling", link: "/implementation/error-handling" },
          {
            text: "Naming Conventions",
            link: "/implementation/naming-conventions",
          },
          {
            text: "File Organization",
            link: "/implementation/file-organization",
          },
        ],
      },
      {
        text: "Advanced Topics",
        items: [{ text: "Deployment", link: "/advanced/deployment" }],
      },
    ],

    socialLinks: [
      { icon: "github", link: "https://github.com/CleanIAM/CleanIAM" },
    ],
  },
});
