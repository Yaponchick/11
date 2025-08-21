[{
	"resource": "/c:/Users/ZhivotovSS/Desktop/Frontend/src/pages/adminPanel/adminPanel.tsx",
	"owner": "typescript",
	"code": "2345",
	"severity": 8,
	"message": "Аргумент типа \"string\" нельзя назначить параметру типа \"number\".",
	"source": "ts",
	"startLineNumber": 270,
	"startColumn": 59,
	"endLineNumber": 270,
	"endColumn": 68
}]

import React, { useState, useEffect } from 'react';
import apiClient from '../../api/apiClient';
import './adminPanelStyle.css';

interface User {
    id: number;
    fullName: string;
    role: string;
}

interface LogEntry {
    id: string;
    timestamp: string;
    employee: string;
    action: string;
}

const AdminPanel = () => {
    const [users, setUsers] = useState<User[]>([]);
    const [admin, setAdmin] = useState<User[]>([]);

    const [logs, setLogs] = useState<LogEntry[]>([]);
    const [search, setSearch] = useState('');
    const [loading, setLoading] = useState<boolean>(false);
    const [error, setError] = useState<string | null>(null);
    const [isModalOpen, setIsModalOpen] = useState<boolean>(false);
    const [isModalOpenList, setIsModalOpenList] = useState<boolean>(false);
    const [isModalOpenChange, setisModalOpenChange] = useState<boolean>(false);
    const [selectedUser, setSelectedUser] = useState('');

    const roleLabels: { [key: number]: string } = {
        1: 'Админ',
        2: 'Создатель',
        3: 'Респонтенд'
    };

    const getRoleLabel = (roleId: number): string => {
        return roleLabels[roleId] || 'Неизвестная роль';
    };

    // Загрузка данных
    useEffect(() => {
        const fetchData = async () => {
            try {
                setLoading(true);
                const [usersRes] = await Promise.all([
                    apiClient.get<User[]>('/user/infoUser'),
                ]);
                const [AdminRes] = await Promise.all([
                    apiClient.get<User[]>('/user/infoAdmin'),
                ]);
                setUsers(usersRes.data);
                setAdmin(AdminRes.data);

            } catch (err: any) {
                console.error('Ошибка загрузки данных:', err);
                setError('Не удалось загрузить данные. Проверьте подключение или права доступа.');
            } finally {
                setLoading(false);
            }
        };

        fetchData();
    }, []);

    // Фильтр по ФИО
    const filteredUsers = users.filter((user) =>
        user.fullName.toLowerCase().includes(search.toLowerCase())
    );

    const filteredAdmin = admin.filter((user) =>
        user.fullName.toLowerCase().includes(search.toLowerCase())
    );

    // Обработка регистрации
    const handleRegister = async (e: React.FormEvent<HTMLFormElement>) => {
        e.preventDefault();
        const form = e.target as HTMLFormElement;
        const fullName = (form.fullName as HTMLInputElement).value.trim();
        const email = (form.email as HTMLInputElement).value.trim();
        const password = (form.password as HTMLInputElement).value;
        const confirmPassword = (form.confirmPassword as HTMLInputElement).value;

        // Очистка ошибок
        setError('');
        setLoading(true);

        if (password !== confirmPassword) {
            setError('Пароли не совпадают.');
            setLoading(false);
            return;
        }

        try {
            await apiClient.post('/auth/register', {
                Username: fullName,
                Email: email,
                Password: password,
                AccessLevelId: 2,
            });

            setIsModalOpen(false);
            setError('');

            // Обновить список пользователей
            const res = await apiClient.get<User[]>('/user/infoUser');
            setUsers(res.data);
        } catch (err: any) {
            console.error('Ошибка регистрации:', err);
            const message =
                err.response?.data?.message ||
                err.response?.data?.error ||
                'Ошибка регистрации. Проверьте данные или попробуйте позже.';
            setError(message);
        } finally {
            setLoading(false);
        }
    };

    if (loading) {
        return (
            <div className="loading-spinner-container">
                <div className="loading-spinner"></div>
                <p>Загрузка данных...</p>
            </div>
        );
    }

    if (error) {
        return (
            <div className="adminPanel-container">
                <div className="error-message">{error}</div>
            </div>
        );
    }

    return (
        <div className="adminPanel-container">
            {/* Модальное окно списка админов */}
            {isModalOpenList && (
                <div className="modal-overlay" onClick={() => setIsModalOpen(false)}>
                    <div className="modal-content" onClick={(e) => e.stopPropagation()}>
                        <h3>Список админов</h3>

                        <table className="surveyAdmin-table">
                            <thead>
                                <tr>
                                    <th>ID</th>
                                    <th>Фамилия И.О.</th>
                                </tr>
                            </thead>
                            {filteredAdmin.map((user) => (

                                <tr key={user.id}>
                                    <td>{user.id}</td>
                                    <td>{user.fullName}</td>

                                </tr>
                            ))}
                        </table>

                        <div className='buttonAdminListContainer'>
                            <button
                                type="button"
                                className="buttonAdminList"
                                onClick={() => setIsModalOpenList(false)}
                            >
                                Закрыть
                            </button>
                        </div>
                    </div>
                </div>
            )}
            {/* Модальное окно регистрации */}
            {isModalOpen && (
                <div className="modal-overlay" onClick={() => setIsModalOpen(false)}>
                    <div className="modal-content" onClick={(e) => e.stopPropagation()}>
                        <form onSubmit={handleRegister}>
                            <div className="form-group-modal">
                                <label>Имя пользователя</label>
                                <input type="text" name="fullName" placeholder="Иванов И.И." required />
                            </div>
                            <div className="form-group-modal">
                                <label>Email</label>
                                <input type="email" name="email" placeholder="example@mail.ru" required />
                            </div>
                            <div className="form-group-modal">
                                <label>Пароль</label>
                                <input type="password" name="password" required minLength={3} />
                            </div>
                            <div className="form-group-modal">
                                <label>Подтвердите пароль</label>
                                <input type="password" name="confirmPassword" required minLength={3} />
                            </div>

                            {error && <p className="error-message">{error}</p>}

                            <div className="modal-actions">
                                <button
                                    type="submit"
                                    className="buttonAdmin"
                                    disabled={loading}
                                >
                                    {loading ? 'Регистрация...' : 'Зарегистрироваться'}
                                </button>
                                <button
                                    type="button"
                                    className="buttonAdmin"
                                    style={{ backgroundColor: '#6c757d' }}
                                    onClick={() => setIsModalOpen(false)}
                                >
                                    Отмена
                                </button>
                            </div>
                        </form>
                    </div>
                </div>
            )}
            {isModalOpenChange && (
                <div className="modal-overlay" onClick={() => setisModalOpenChange(false)}>
                    <div className="modal-content" onClick={(e) => e.stopPropagation()}>
                        {selectedUser}
                    </div>
                </div>

            )}


            <div className="adminPanel-inner-container">
                {/* Таблица пользователей */}
                <div className="tableAdmin-container">
                    <h2 className="headingText">Список пользователей</h2>
                    <div className='filterAdminContainer'>
                        <div className="filterAdmin">
                            <input
                                type="text"
                                placeholder="Поиск по ФИО"
                                value={search}
                                onChange={(e) => setSearch(e.target.value)}
                                className="inputSearch"
                                aria-label="Поиск по ФИО"
                            />
                            <button
                                className="buttonAdmin"
                                onClick={() => setIsModalOpen(true)}
                            >
                                Зарегистрировать
                            </button>
                            <button onClick={() => setIsModalOpenList(true)}
                                className="buttonAdmin">
                                Список админов
                            </button>
                        </div>
                    </div>
                    <table className="surveyAdmin-table">
                        <thead>
                            <tr>
                                <th>ID</th>
                                <th>Фамилия И.О.</th>
                                <th>Роль</th>
                                <th>Действие</th>
                            </tr>
                        </thead>
                        <tbody>
                            {filteredUsers.length > 0 ? (
                                filteredUsers.map((user) => (
                                    <tr key={user.id}>
                                        <td>{user.id}</td>
                                        <td>{user.fullName}</td>
                                        <td>{getRoleLabel(user.role)}</td>
                                        <td>
                                            <div className="dropdown">
                                                <button className="action-btn" aria-label="Действия">
                                                    ⋯
                                                </button>
                                                <div className="dropdown-content">
                                                    <div className="context-button">

                                                        <button className="menu-item"
                                                            onClick={() => {

                                                                setSelectedUser(user.fullName);
                                                                setisModalOpenChange(true)
                                                            }}>
                                                            Изменить роль
                                                        </button>

                                                        {/* <button
                                                            className="menu-item"
                                                            onClick={async () => {
                                                                const newRole = prompt(
                                                                    `Введите новую роль: Admin, Creator или Respondent`,
                                                                    user.role
                                                                );

                                                                if (!newRole) return;

                                                                try {
                                                                    await apiClient.put(`/user/${user.id}/role`, { role: newRole });
                                                                    alert('Роль обновлена');

                                                                } catch (err: any) {
                                                                    const message =
                                                                        err.response?.data?.message ||
                                                                        'Ошибка изменения роли. Проверьте название роли.';
                                                                    alert(message);
                                                                }
                                                            }}
                                                        >
                                                            Изменить роль
                                                        </button> */}




                                                        <button
                                                            className="menu-item danger"
                                                            onClick={async () => {
                                                                if (!window.confirm(`Удалить пользователя ${user.fullName}?`)) return;

                                                                try {
                                                                    await apiClient.delete(`/auth/delete/${user.id}`);
                                                                    setUsers(users.filter(u => u.id !== user.id));
                                                                    alert('Пользователь удалён');
                                                                } catch (err: any) {
                                                                    const message = err.response?.data?.message || 'Ошибка удаления';
                                                                    alert(message);
                                                                }
                                                            }}
                                                        >
                                                            Удалить
                                                        </button>
                                                    </div>
                                                </div>
                                            </div>
                                        </td>
                                    </tr>
                                ))
                            ) : (
                                <tr>
                                    <td colSpan={3} className="no-data">
                                        Пользователи не найдены
                                    </td>
                                </tr>
                            )}
                        </tbody>
                    </table>
                </div>
            </div>
        </div>
    );
};

export default AdminPanel;
