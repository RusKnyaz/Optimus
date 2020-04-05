# Optimus Changelog
All notable changes to this project will be documented in this file.

## [3.0.0]

### Breaking changes
- Removed Engine(IResourceProvider) constructor. Use EngineBuilder.
- Removed Engine.OnWindowOpen event. Use EngineBuilder to configure Window object.


## [2.4]

### New features
- Expanded TypedArray API
- Expanded Console API (Added new methods: info, warn, assert, group, ...)


## [2.3] - 2019-11-23

### New features
- NavigatorPlugins
- EngineBuilder - new way to configure optimus engine
- EngineBuilder.JsScriptExecutor - ability to specify own js script execution engine
- Cookie container properties

## [2.2.6]

### Added 
- HtmlImageElement.ImageData property to get raw image data
### Fixed
- Loading of image when source specified using setAttribute('src', ...) method.
- Execution addEventListener("", null)

## [2.2] - 2017-08-20

### Added
- Ability to specify UserAgent
- Using of web proxies
- Basic authorization
- Pre-handling of http requests
- Status code of opened page
- Added support of overloads of setTimeout/setInterval with arbitrary count of arguments
- Added session storage API
- HtmlImageElement


### Changed
- Fixed bug with decoding of base64 from data: url
- Fixed agent's styles priority
- Fixed dead-lock on script execution


## [2.1] - 2017-02-16

Fixed bugs and extended DOM API.

### Added
- Documentation
- Methods and properties of HtmlButtonElement, HtmlInputElement, HtmlOptionElement
- HtmlOptionGroupElement

### Changed
- Fixed events propogation and handling
- Downgraded Jint to stable version
- Fixed 'this' value inside event handlers.

## [2.0] - 2017-12-xx

### Added
- Netstandard and Netcore support
- Xml documentation

### Changed
- Some changes in API (hide element's constructors, fixed names and etc.)
- Fixed loading resources from relative path
- Fixed DOM manipulation methods.
- Fixed some minor bugs.

## [1.2] 2017-06-xx

### Added
- 'self' global variable now available.

### Changed
- CSS evaluation improved
- Selectors fixed
- Fixed parsing of unclosed tags.
