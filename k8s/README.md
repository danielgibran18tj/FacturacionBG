# Despliegue de una aplicación de frontend y backend en Kubernetes

## Prerequisitos

- Tener instalado Kubernetes
- Tener instalado Docker
- Tener instalado kubectl
- Minikube instalado

## Configuracion de minikube para tomar imagenes de registry local

minikube start --driver=docker

eval $(minikube docker-env)

docker ps

docker-compose build

docker-compose up   


#### Para correr nuestros .yaml

## Backend

kubectl apply -f k8s/backend

## Frontend

kubectl apply -f k8s/frontend


## Comprobar la correcta creacion 

kubectl get pods,service,deploy

En caso de que haya error 'ErrImageNeverPull', usar 
```bash
# Opción A: construir en contexto minikube (invisible en Docker Desktop)
eval $(minikube docker-env)
docker-compose build

# Opción B: construir normal y luego cargar (visible en Docker Desktop)
minikube image load backend:latest
```
antes de crear los contendores para Apuntar tu Docker CLI al daemon de minikube

# OJO
Si intentas acceder a un servicio tipo NodePort usando la IP que te da el tunnel, no funcionará, porque el tunnel solo "escucha" servicios que Kubernetes ha marcado específicamente como LoadBalancer.

Los servicios NodePort se exponen con 
```bash
minikube service frontend-service
```

Los servicios LoadBalancer se exponen con 
```bash
minikube tunnel
```
debido a que cuando creas un servicio LoadBalancer, el proveedor de la nube real (como AWS o Azure) te da una IP pública externa. En tu computadora (local), no hay un proveedor de nube que asigne IPs externas.


minikube addons enable metrics-server
