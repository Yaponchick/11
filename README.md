const handleUpdate = async () => {
    if (!surveyId || isLoading) return;
    setIsLoading(true);

    try {
        // 1. Обновляем заголовок анкеты
        await typedApiClient.put(`/questionnaire/${surveyId}/title`, { 
            NewTitle: title 
        });

        const questionsToProcess = questions.filter(q => !q.isDeleting);
        const questionOrder: string[] = [];
        const backendQuestionIds: string[] = [];

        // 2. Обрабатываем каждый вопрос
        for (const question of questionsToProcess) {
            const isNewQuestion = !question.id;
            let backendId = question.id;
            const questionTypeId = questionTypeMapping[question.type as keyof typeof questionTypeMapping];
            const questionTextPayload = (question.type === "Шкала")
                ? `${question.text || ''}|${question.leftScaleValue || ""}|${question.rightScaleValue || ""}|${question.divisions || 5}`
                : question.text;

            // 2.1. Создаем или обновляем вопрос
            if (isNewQuestion) {
                const response = await typedApiClient.post(`/questionnaire/${surveyId}/questions/add-question`, {
                    Text: questionTextPayload, 
                    QuestionType: questionTypeId,
                });
                backendId = response.data.questionId;
            } else if (backendId) {
                await typedApiClient.put(`/questionnaire/${surveyId}/questions/${backendId}/text`, { 
                    NewText: questionTextPayload 
                });
                await typedApiClient.put(`/questionnaire/${surveyId}/questions/${backendId}/type`, { 
                    NewQuestionType: questionTypeId 
                });
            }

            // Сохраняем ID для обновления порядка
            if (backendId) {
                questionOrder.push(backendId);
                backendQuestionIds.push(backendId);
            }

            // 2.2. Обрабатываем ответы вопроса
            if (["Закрытый", "Множественный выбор", "Выпадающий список"].includes(question.type) && backendId) {
                for (const answer of question.answers) {
                    if (answer.isDeleting && answer.id) {
                        await typedApiClient.delete(`/questionnaire/${surveyId}/questions/${backendId}/options/${answer.id}`);
                    } else if (!answer.id && !answer.isDeleting && answer.text.trim()) {
                        await typedApiClient.post(`/questionnaire/${surveyId}/questions/${backendId}/options`, { 
                            OptionText: answer.text 
                        });
                    } else if (answer.id && !answer.isDeleting && answer.text.trim()) {
                        await typedApiClient.put(`/questionnaire/${surveyId}/questions/${backendId}/options/${answer.id}`, { 
                            NewOptionText: answer.text 
                        });
                    }
                }
            }
        }

        // 3. Обновляем порядок вопросов (только если есть вопросы с backend ID)
        if (questionOrder.length > 0) {
            try {
                await typedApiClient.put(`/questionnaire/${surveyId}/questions/order`, 
                    questionOrder
                );
            } catch (orderError: any) {
                console.warn('Не удалось обновить порядок вопросов, но анкета сохранена:', orderError);
                // Продолжаем выполнение, так как основное содержимое сохранено
            }
        }

        // 4. Удаляем вопросы, помеченные на удаление
        for (const deletedId of deletedQuestionIds) {
            try {
                await typedApiClient.delete(`/questionnaire/${surveyId}/questions/${deletedId}`);
            } catch (deleteError: any) {
                console.warn(`Не удалось удалить вопрос ${deletedId}:`, deleteError);
                // Продолжаем выполнение
            }
        }

        // 5. Очищаем список удаленных ID
        setDeletedQuestionIds([]);
        
        // 6. Показываем успешное сообщение и перенаправляем
        alert('Анкета успешно сохранена!');
        navigate('/account');

    } catch (err: any) {
        console.error('Ошибка при сохранении анкеты:', err);
        const errorMessage = err.response?.data?.message || err.message || 'Неизвестная ошибка';
        setError(`Ошибка при сохранении: ${errorMessage}`);
        
        // Показываем ошибку пользователю
        alert(`Ошибка сохранения: ${errorMessage}`);
    } finally {
        setIsLoading(false);
    }
};
