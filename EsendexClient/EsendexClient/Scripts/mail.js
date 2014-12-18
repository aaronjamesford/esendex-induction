(function() {
    var padNumber = function(n) {
        if (n < 10) {
            return "0" + n;
        }

        return n;
    }

    var formatDate = function(d) {
        return padNumber(d.getDate()) + "/" + padNumber(d.getMonth() + 1) + "/" + padNumber(d.getFullYear()) + " " + padNumber(d.getHours()) + ":" + padNumber(d.getMinutes());
    }


    var app = angular.module("esendex-mail", [])
        .directive("drawConversationDirective", function() {
            return function(scope) {
                if (scope.$last) {
                    $(function() {
                        $('[data-toggle="tooltip"]').tooltip();
                    });
                }
            }
        });
    
    app.controller("MailboxController", function($scope, $http) {
        $scope.accountReference = "EX0153074";
        $scope.accountVmn = "447860025149";
        
        $scope.conversations = [];

        $scope.getConversations = function() {
            $http.get("/api/Conversation")
                .success(function(data) {
                    $scope.conversations = data;

                    for (var i = 0; i < data.length; ++i) {
                        data[i].messages = [
                             { message: data[i].summary, direction: "outbound", lastEvent: "10/07/2014 16:28", status: "Delivered" }
                        ];
                    }

                    //setTimeout($scope.getConversations, 5000);
                }).error(function() {
                    alert("There was an error retreiving messages");
                });
        }
            
        $scope.setActiveConversation = function (participant) {
            if ($scope.activeConversation !== undefined) {
                for (var idx = 0; idx < $scope.conversations.length; ++idx) {
                    var conversation = $scope.conversations[idx];
                    if (conversation.participant == $scope.activeConversation.participant) {
                        $scope.conversations[idx].active = false;
                        break;
                    }
                }
            }

            $scope.activeConversation = undefined;

            for(var idx = 0; idx < $scope.conversations.length; ++idx) {
                var conversation = $scope.conversations[idx];
                if(conversation.participant == participant) {
                    $scope.conversations[idx].active = true;

                    $http.get("/api/conversation/?participant=" + participant)
                        .success(function (data) {
                            data.participant = participant;

                            for (var i = 0; i < data.length; ++i) {
                                var d = new Date(data[i].lastStatusAt);
                                data[i].lastStatusAt = formatDate(d);
                            }

                            $scope.activeConversation = data;
                        });
                    
                    break;
                }
            }
            
            return true;
        };
    });
})();