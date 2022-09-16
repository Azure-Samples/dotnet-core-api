'use strict';
angular.module('todoApp')
.factory('todoListSvc', ['$http', function ($http) {

    $http.defaults.useXDomain = true;
    delete $http.defaults.headers.common['X-Requested-With']; 

    return {
        getItems : function(){
            return $http.get(apiEndpoint + '/api/todo');
        },
        getItem : function(id){
            return $http.get(apiEndpoint + '/api/todo/' + id);
        },
        postItem : function(item){
            return $http.post(apiEndpoint + '/api/todo', item);
        },
        putItem : function(item){
            return $http.put(apiEndpoint + '/api/todo/' + item.id, item);
        },
        deleteItem : function(id){
            return $http({
                method: 'DELETE',
                url: apiEndpoint + '/api/todo/' + id
            });
        }
    };
}]);
