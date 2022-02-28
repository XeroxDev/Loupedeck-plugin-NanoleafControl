// .versionrc.js
const tracker = [
    {
        filename: './NanoleafControlPlugin/Properties/AssemblyInfo.cs',
        updater: require('./standard-version-updater/AssemblyInfo.js')
    },
    {
        filename: './package.json',
        type: 'json'
    },
    {
        filename: './LoupedeckPackage.yaml',
        updater: require.resolve("standard-version-updater-yaml")
    }
]

module.exports = {
    sign: true,
    commitAll: true,
    bumpFiles: tracker,
    packageFiles: tracker
}