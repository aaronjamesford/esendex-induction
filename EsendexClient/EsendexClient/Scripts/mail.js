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

    var toUtc = function (d) {
        return padNumber(d.getFullYear()) + "-" + padNumber(d.getMonth() + 1) + "-" + padNumber(d.getDate()) + "T" + padNumber(d.getHours()) + ":" + padNumber(d.getMinutes()) + ":" + padNumber(d.getSeconds()) + ".000+00:00";
    }

    var apiControllers = {
        rest : {
            conversation: "/api/Conversation",
            message: "/api/Message",
            account: "/api/EsendexAccount"
        },
        soap : {
            conversation: "/api/Conversation",
            message: "/api/SoapMessage",
            account: "/api/EsendexAccount"
        },
        formPost : {
            conversation: "/api/Conversation",
            message: "/api/FormPostMessage",
            account: "/api/EsendexAccount"
        }
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
        $scope.apiType = "rest";

        $scope.getConversations = function() {
            $http.get(apiControllers[$scope.apiType].conversation)
                .success(function (data) {
                    $scope.conversations = data;
                }).error(function() {
                    alert("There was an error retreiving messages");
                });
        }

        $scope.updateConversationSummary = function (summary) {
            var conversationFound = false;

            for (var i = 0; i < $scope.conversations.length && !conversationFound; ++i) {
                if ($scope.conversations[i].participant == summary.participant) {
                    summary.active = $scope.conversations[i].active;
                    $scope.conversations[i] = summary;
                    
                    conversationFound = true;
                }
            }

            if (!conversationFound) {
                $scope.conversations.push(summary);
            }
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

        $scope.useRest = function() {
            $scope.apiType = "rest";
        }

        $scope.useSoap = function () {
            $scope.apiType = "soap";
        }

        $scope.useFormPost = function () {
            $scope.apiType = "formPost";
        }

        $scope.conversationHub.client.onUpdatedConversation = function(message) {
            $scope.updateConversationSummary(message);
            $scope.$apply();
        }

        $scope.conversationHub.client.onMessageFailed = function (message) {
            if ($scope.activeConversation !== undefined) {
                for (var i = 0; i < $scope.activeConversation.length; ++i) {
                    if ($scope.activeConversation[i].id == message.messageId) {
                        $scope.activeConversation[i].lastStatusAt = formatDate(new Date(message.occurredAt));
                        $scope.activeConversation[i].status = "Failed";

                        $scope.$apply();
                        break;
                    }
                }
            }
        }

        $scope.conversationHub.client.onMessageDelivered = function (message) {
            if ($scope.activeConversation !== undefined) {
                for (var i = 0; i < $scope.activeConversation.length; ++i) {
                    if ($scope.activeConversation[i].id == message.messageId) {
                        $scope.activeConversation[i].lastStatusAt = formatDate(new Date(message.occurredAt));
                        $scope.activeConversation[i].status = "Delivered";

                        $scope.$apply();
                        break;
                    }
                }
            }
        }

        $scope.registerInboundMessages = function () {
            $http.get(apiControllers[$scope.apiType].account).success(function (account) {
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

                    $http.get(apiControllers[$scope.apiType].conversation + "?participant=" + participant)
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

            var body = this.body;

            $http.post(apiControllers[$scope.apiType].message, { to: participant, body: body })
                .success(function (data) {
                    var now = new Date();

                    $scope.activeConversation.push({
                        body: body,
                        status: "Submitted",
                        lastStatusAt: formatDate(now),
                        direction: "Outbound",
                        id: data.messageId
                    });

                    var summary = body.length > 50 ? body.substr(0, 57) + "..." : body;

                    $scope.updateConversationSummary({
                        participant: participant,
                        active: true,
                        summary: summary,
                        lastMessageAt: toUtc(now)
                    });

                    $('#body-text').removeAttr('disabled').removeClass('disabled').text('').val('');
                    $('#body-submit').removeAttr('disabled').removeClass('disabled');
                });

            
        }
    });

    $(document).ready(function() {
        $("#settings-dropdown").on("show.bs.dropdown", function() { $("#settings-dropdown-button").addClass("clicked"); });
        $("#settings-dropdown").on("hide.bs.dropdown", function() { $("#settings-dropdown-button").removeClass("clicked"); });
    });
})();