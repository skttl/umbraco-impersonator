import { defineConfig } from "vite";

export default defineConfig({
  build: {
    lib: {
      entry: "src/impersonator-app.ts", // your web component source file
      formats: ["es"],
    },
    outDir: "../wwwroot", // your web component will be saved in this location
    sourcemap: true,
    rollupOptions: {
      external: [/^@umbraco/],
    },
  },
});