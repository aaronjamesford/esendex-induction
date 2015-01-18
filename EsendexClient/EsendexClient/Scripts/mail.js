(function () {
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
                        $("#conversation").scrollTop($("#conversation")[0].scrollHeight);

                    $(function() {
                        $('[data-toggle="tooltip"]').tooltip();
                    });
                }
            }
        });
    
    app.controller("MailboxController", function($scope, $http) {
        $scope.conversations = [];

        $scope.getConversations = function() {
            $http.get("/api/Conversation")
                .success(function(data) {
                    $scope.conversations = data;
                }).error(function() {
                    alert("There was an error retreiving messages");
                });
        }

        $scope.conversationHub = $.connection.conversationHub;
        $scope.conversationHub.client.onInboundMessage = function (message) {
            if ($scope.activeConversation !== undefined && $scope.activeConversation.participant == message.from.phoneNumber) {
                var d = new Date(message.lastStatusAt);
                message.lastStatusAt = formatDate(d);

                $scope.activeConversation.push(message);
                $scope.$apply();
            }
        }

        $scope.registerInboundMessages = function () {
            $http.get("/api/EsendexAccount").success(function (account) {
                $.connection.hub.url = "/signalr";
                $.connection.hub.start().done(function () {
                    $scope.conversationHub.server.register(account.id);
                });
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

        $scope.sendMessage = function (participant) {
            if (!participant) return;

            $('#body-text').attr('disabled', '').addClass('disabled');
            $('#body-submit').attr('disabled', '').addClass('disabled');
            
            $http.post('/api/Message', { to: participant, body: this.body })
                .success(function (data) {
                    $scope.setActiveConversation(participant);
                    $('#body-text').removeAttr('disabled').removeClass('disabled').text('').val('');
                    $('#body-submit').removeAttr('disabled').removeClass('disabled');
                });
        }
    });
})();