var pollName = window.location.pathname.split('/').filter(function (x) { return x !== ""; }).pop();
var app = angular.module('VotingApp', ['ui.bootstrap']);
app.run(function () { });

app.controller('VotesController', ['$rootScope', '$scope', '$http', '$timeout', function ($rootScope, $scope, $http, $timeout) {

    $scope.refresh = function () {
        $http.get('/api/Votes/' + pollName + '?c=' + new Date().getTime())
            .then(function (data, status) {
                $scope.votes = data;
            }, function (data, status) {
                $scope.votes = undefined;
            });
    };

    $scope.remove = function (item) {
        $http.delete('/api/Votes/' + pollName + '/' + item)
            .then(function (data, status) {
                $scope.refresh();
            });
    };

    $scope.add = function (item) {
        var fd = new FormData();
        fd.append('item', item);
        $http.put('/api/Votes/' + pollName + '/' + item, fd, {
            transformRequest: angular.identity,
            headers: { 'Content-Type': undefined }
        })
            .then(function (data, status) {
                $scope.refresh();
                $scope.item = undefined;
            });
    };
}]);


app.controller('IndexController', ['$rootScope', '$scope', '$http', '$timeout', function ($rootScope, $scope, $http, $timeout) {

    $scope.refresh = function () {
        $http.get('/api/polls/' + '?c=' + new Date().getTime())
            .then(function (data, status) {
                $scope.polls = data;
            }, function (data, status) {
                $scope.polls = undefined;
            });
    };

    $scope.remove = function (item) {
        $http.delete('/api/polls/' + item)
            .then(function (data, status) {
                $scope.refresh();
                $scope.item = undefined;
            });
    };

    $scope.add = function (item) {
        $http.put('/api/polls/' + item)
            .then(function (data, status) {
                $scope.refresh();
                $scope.item = undefined;
            });
    };

    $scope.open = function (item) {
        location.href = "/polls/" + item;
    };
}]);