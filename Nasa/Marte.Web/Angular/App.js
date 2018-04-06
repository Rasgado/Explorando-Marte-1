'use strict';

var app = angular.module("mensagemApp", []);

app.controller("mensagemCtrl", function ($scope, $http) {

    $scope.conteudo = "";
    $scope.mensagemSucesso1 = "";
    $scope.mensagemSucesso2 = "";
    $scope.mensagemErro = "";

    $scope.enviarComando = function (conteudo) {

        if (!conteudo) {
            $scope.mensagemErro = "Campo conteúdo dos comandos é obrigatório o preenchimento.";
        } else {
            $scope.mensagemErro = "";
            $scope.mensagemSucesso1 = "Comando enviado, aguarde o retorno.";
            $scope.mensagemSucesso2 = "";

            $http.post("http://localhost:56476/api/Mensagem", { conteudo }).then(
                function (success) {
                    var itens = success.data.split("-");
                    $scope.mensagemSucesso1 = itens[0];
                    $scope.mensagemSucesso2 = itens[1];
                }, function (error) {
                    $scope.mensagemSucesso1 = "";
                    $scope.mensagemErro = error.data.errors;
                });

        };

    }

});