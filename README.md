.alert {
  position: fixed;
  top: 20px;
  right: 20px;
  max-width: 350px;
  border-radius: 8px;
  padding: 12px 16px;
  box-shadow: 0 4px 12px rgba(0, 0, 0, 0.15);
  display: flex;
  justify-content: space-between;
  align-items: center;
  font-size: 14px;
  z-index: 1000;
  animation: slideIn 0.3s ease-out;
}

.alert-success {
  background-color: #d4edda;
  color: #155724;
  border: 1px solid #c3e6cb;
}

.close-icon {
  cursor: pointer;
  font-weight: bold;
  margin-left: 10px;
  opacity: 0.7;
  font-size: 18px;
  width: 20px;
  height: 20px;
  display: flex;
  align-items: center;
  justify-content: center;
}

.close-icon:hover {
  opacity: 1;
}

@keyframes slideIn {
  from {
    opacity: 0;
    transform: translateX(100%);
  }
  to {
    opacity: 1;
    transform: translateX(0);
  }
}
