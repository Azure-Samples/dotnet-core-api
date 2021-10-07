'use strict';
angular.module('todoApp', ['ngRoute'])
.config(['$routeProvider', '$httpProvider', function ($routeProvider, $httpProvider) {

    // disable IE ajax request caching
    if (!$httpProvider.defaults.headers.get) {
        $httpProvider.defaults.headers.get = {};
    }
    $httpProvider.defaults.headers.get['If-Modified-Since'] = '0';

    $routeProvider.when("/Home", {
        controller: "todoListCtrl",
        templateUrl: "/app/views/TodoList.html",
    }).otherwise({ redirectTo: "/Home" });

    }]);
