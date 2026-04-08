const { defineConfig } = require('@vue/cli-service')
module.exports = defineConfig({
  transpileDependencies: true,
  outputDir: "../Scripts/client-app/",
  publicPath: "/DesktopModules/UpendoBusinessDirectoryManager/Scripts/client-app/",
  filenameHashing: false
})
