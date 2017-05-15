(function () {
    var app = angular.module('app', ['ui.router', 'ngRoute']);
    app.config(['$stateProvider', '$urlRouterProvider', '$locationProvider', '$routeProvider', function ($stateProvider, $urlRouterProvider, $locationProvider, $routeProvider) {

        $urlRouterProvider.otherwise('/first-page');

        $stateProvider.state("firstPage", { url: '/first-page', templateUrl: 'App/firstPage.html', controller: 'firstPageController' });
        $stateProvider.state("secondPage", { url: '/second-page', templateUrl: 'App/secondPage.html', controller: 'secondPageController' });
        $stateProvider.state("thirdPage", { url: '/third-page', templateUrl: 'App/thirdPage.html', controller: 'thirdPageController' });
        $stateProvider.state("thirdPage.firstNested", { url: '/first-nested', templateUrl: 'App/firstNestedPage.html', controller: 'firstNestedPageController' });
        $stateProvider.state("thirdPage.secondNested", { url: '/second-nested', templateUrl: 'App/secondNestedPage.html', controller: 'secondNestedPageController' });
        $stateProvider.state("leaderboard", { url: '/leaderboard/:tournamentId', templateUrl: 'App/LeaderboardPage.html', controller: 'leaderboardController' });
        $stateProvider.state("final-leaderboard", { url: '/final-leaderboard/:tournamentId', templateUrl: 'App/finalLeaderboardPage.html', controller: 'leaderboardController' });
        $stateProvider.state("edit-golfer", { url: '/edit-golfer/:golferId', templateUrl: 'App/editGolfer.html', controller: 'golferController' });
        $stateProvider.state("edit-tournament", { url: '/edit-tournament/:tournamentId', templateUrl: 'App/editTournament.html', controller: 'tournamentController' });
        $stateProvider.state("tournament-scores", { url: '/tournament-scores/:tournamentId', templateUrl: 'App/scoresPage.html', controller: 'leaderboardController' });
        $locationProvider.html5Mode(true);
    }]);

    app.controller("rootController", ["$scope", rootController]);
    app.controller("firstPageController", ["$scope", "firstPageService", firstPageController]);
    app.controller("secondPageController", ["$scope", "secondPageService", secondPageController]);
    app.controller("thirdPageController", ["$scope", "thirdPageService", thirdPageController]);
    app.controller("leaderboardController", ["$scope", "$state", "leaderboardService", leaderboardController]);
    app.controller("firstNestedPageController", ["$scope", firstNestedPageController]);
    app.controller("secondNestedPageController", ["$scope", secondNestedPageController]);
    app.controller("golferController", ["$scope", "$state", "golferService", "golferRoundsService", golferController]);
    app.controller("tournamentController", ["$scope", "$state", "tournamentService", "secondPageService", "thirdPageService",  "golferService", tournamentController]);

    function rootController($scope) {

    }

    function firstPageController($scope, firstPageService) {
        firstPageService.getTournament(function (response) {
            $scope.tournaments = response.data;
        });
    }
       
    function secondPageController($scope, secondPageService) {
        secondPageService.getCourses(function (response) {
            $scope.courses = response.data;
        });
    }

    function thirdPageController($scope, thirdPageService) {
        thirdPageService.getGolfers(function (response) {
            $scope.golfers = response.data;
        });
    }

    function leaderboardController($scope, $state, leaderboardService) {
        var tournamentId = $state.params.tournamentId;
        leaderboardService.getLeaderboard(tournamentId, function (response) {
            $scope.rounds = response.data;
        });
    }

    function golferController($scope, $state, golferService, golferRoundsService) {
        var golferId = $state.params.golferId;
    
        golferService.getGolferInfo(golferId, function (response) {
            $scope.golfer = response.data;
        });

        golferRoundsService.getRounds(golferId, function (response) {
            $scope.golferRounds = response.data;
        });

        $scope.saveGolferClick = function () {
            golferService.updateGolferInformation($scope.golfer, function (response) {

            });
        }

        $scope.calculateHandicapClick = function () {
                golferService.calculateGolferHandicap($scope.golfer, function (response) {
                    $scope.golfer = response.data;
                });
            
            }
        }

    function tournamentController($scope, $state, tournamentService, secondPageService, thirdPageService, golferService) {
        var tournamentId = $state.params.tournamentId;
        

        tournamentService.getTournamentInfo(tournamentId, function (response) {
            $scope.tournament = response.data;
        });
        secondPageService.getCourses(function (response) {
            $scope.courses = response.data;
        });

        thirdPageService.getGolfers(function (response) {
            $scope.golfers = response.data;
        });
                                            
        $scope.saveTournamentClick = function () {
            tournamentService.updateTournamentInformation($scope.tournament, function (response) {

            });
        }

        $scope.addTournamentClick = function () {
            tournamentService.addTournamentInfo(function (response) {

            });
        }

        $scope.addBlankTournamentRoundClick = function () {
            tournamentService.createBlankTournamentRound($scope.golfer, $scope.tournament, function (response) {

            });
        }
    }

    function firstNestedPageController($scope) {
    }

    function secondNestedPageController($scope) {
    }


    app.factory('firstPageService', ['$http', function ($http) {
        return {
            getTournament: function (success, error) {
                return $http.get('/home/loadTournaments/1').then(success, error);
            }
        };
    }]);

    app.factory('secondPageService', ['$http', function ($http) {
        return {
            getCourses: function (success, error) {
                return $http.get('/home/loadCourses/1').then(success, error);
            }
        };
    }]);

    app.factory('thirdPageService', ['$http', function ($http) {
        return {
            getGolfers: function (success, error) {
                return $http.get('/home/loadGolfers/1').then(success, error);
            }
        };
    }]);

    app.factory('leaderboardService', ['$http', '$state', function ($http, $state) {
        return {
            getLeaderboard: function (tournamentId, success, error) {
                return $http.get('/home/loadRounds/' + tournamentId).then(success, error);
            }
        };
    }]);

  
    app.factory('golferService', ['$http', '$state', function ($http, $state) {
        return {
            getGolferInfo: function (golferId, success, error) {
                return $http.get('/home/displayGolferInfo/' + golferId).then(success, error);
            },
            updateGolferInformation: function (golfer, success, error) {
                return $http.post('/home/updateGolfer/', golfer).then(success, error);
            }
        }
    }]);

       app.factory('golferRoundsService', ['$http', '$state', function ($http, $state) {
        return {
            getRounds: function (golferId, success, error) {
                return $http.get('/home/loadGolferRounds/' + golferId).then(success, error);
            }
        };
    }]);

    app.factory('tournamentService', ['$http', '$state', function ($http, $state) {
        return {
            getTournamentInfo: function (tournamentId, success, error) {
                return $http.get('/home/editTournament/' + tournamentId).then(success, error);
            },
            updateTournamentInformation: function (tournament, success, error) {
                return $http.post('/home/updateTournament/', tournament).then(success, error);
            },
            addTournamentInfo: function (success, error) {
                return $http.get('/home/editTournament/').then(success, error);
            },
            createBlankTournamentRound: function (golfer, tournament, success, error) {
                return $http.post('/home/createBlankRound/', golfer, tournament).then(success, error);
            },
            deleteBlankTournamentRound: function (golferId, tournament, success, error) {
                return $http.post('/home/deleteBlankRound/' + golferId + tournament).then(success, error);
            }
        };
    }]);

    app.factory('finalLeaderboardService', ['$http', '$state', function ($http, $state) {
            return {
            getFinalLeaderboard: function (tournamentId, success, error) {
                return $http.get('/home/loadFinalRounds/' + tournamentId).then(success, error);
            }
        };
    }]);
    

})();