"use strict";
var page = require('webpage').create();
var system = require('system');
var pageUrl = '';
var imageName = '';

pageUrl = system.args[1];
imageName = system.args[2];

page.open(pageUrl, function () {
    //http://phantomjs.org/api/webpage/method/render.html
    //render(filename [, {format, quality}]) {void}
    //page.render(imageName, {
    //    format: 'png', quality: '20'
    //});

    page.render(imageName);
    phantom.exit();

});
