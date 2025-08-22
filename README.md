import { useState, useEffect } from 'react';
import axios from 'axios';
import Cookies from 'js-cookie';
import { useParams, useNavigate } from 'react-router-dom';

interface Author {
  firstName: string;
  lastName: string;
}

interface Option {
  id: number;
  optionText: string;
  order: number;
}

export interface Question {
  id: number;
  text: string;
  questionTypeId: number;
  options?: Option[];
  leftScaleValue?: string;
  rightScaleValue?: string;
  divisions?: number;
}

interface QuestionnaireData {
  title: string;
  questions: Question[];
  author: Author;
}

export type AnswerValue = string | number | number[] | null;

export interface ValidationErrors {
  [key: number]: string;
}

interface PopupState {
  visible: boolean;
  text: string;
  x: number;
  y: number;
}

export const useAnswersLogic = () => {
  const { id } = useParams<{ id: string }>();
  const navigate = useNavigate();

  const [author, setAuthor] = useState<Author>({ firstName: '', lastName: '' });
  const [ansTitle, setAnsTitle] = useState<string>('');
  const [questions, setQuestions] = useState<Question[]>([]);
  const [answers, setAnswers] = useState<{ [key: number]: AnswerValue }>({});
  const [isLoading, setIsLoading] = useState<boolean>(true);
  const [apiError, setApiError] = useState<string>('');
  const [validationErrors, setValidationErrors] = useState<ValidationErrors>({});
  const [popup, setPopup] = useState<PopupState>({ visible: false, text: '', x: 0, y: 0 });

  useEffect(() => {
    const loadQuestionnaire = async () => {
      if (!id) {
        setApiError('Идентификатор анкеты не указан.');
        setIsLoading(false);
        return;
      }

      const token = localStorage.getItem('access_token');

      try {
        const response = await axios.get<QuestionnaireData>(
          `https://localhost:7109/questionnaire/access/${id}`,
          {
            headers: { Authorization: `Bearer ${token}` },
          }
        );

        // Сохраняем SessionId в куки
        const sessionId = response.headers['x-session-id'];
        if (sessionId) {
          Cookies.set('anonymousSessionId', sessionId, { expires: 365, sameSite: 'Lax' });
        }

        const { title, questions: fetchedQuestions, author } = response.data;

        setAuthor(author || { firstName: '', lastName: '' });
        setAnsTitle(title);

        const processedQuestions = fetchedQuestions.map(q => {
          if (q.questionTypeId === 4) {
            const parts = q.text.split('|');
            return {
              ...q,
              text: parts[0] || q.text,
              leftScaleValue: parts[1] || 'Низкое',
              rightScaleValue: parts[2] || 'Высокое',
              divisions: parseInt(parts[3], 10) || 5,
            };
          }
          return q;
        });

        setQuestions(processedQuestions);

        const initialAnswers: { [key: number]: AnswerValue } = {};
        processedQuestions.forEach(q => {
          if (q.questionTypeId === 3) initialAnswers[q.id] = [];
          else if (q.questionTypeId === 4) initialAnswers[q.id] = Math.ceil((q.divisions || 5) / 2);
          else initialAnswers[q.id] = '';
        });
        setAnswers(initialAnswers);

        setIsLoading(false);
      } catch (err: any) {
        if (err.response?.status === 403) {
          navigate('/AlreadySubmitted');
        } else {
          setApiError(err.response?.data || 'Ошибка загрузки анкеты.');
        }
        setIsLoading(false);
      }
    };

    loadQuestionnaire();
  }, [id, navigate]);

  const validateAnswers = (): boolean => {
    const errors: ValidationErrors = {};
    let firstErrorId: number | null = null;

    for (const q of questions) {
      const answer = answers[q.id];
      let isEmpty = false;
      let errorMsg = 'Ответьте на вопрос';

      switch (q.questionTypeId) {
        case 1:
          isEmpty = !answer || !String(answer).trim();
          errorMsg = 'Заполните поле';
          break;
        case 2:
        case 5:
          isEmpty = answer == null || answer === '';
          errorMsg = 'Выберите один вариант';
          break;
        case 3:
          isEmpty = !Array.isArray(answer) || answer.length === 0;
          errorMsg = 'Выберите хотя бы один';
          break;
        case 4:
          isEmpty = isNaN(Number(answer));
          errorMsg = 'Выберите значение';
          break;
      }

      if (isEmpty) {
        errors[q.id] = errorMsg;
        if (firstErrorId === null) firstErrorId = q.id;
      }
    }

    setValidationErrors(errors);
    if (firstErrorId && document.getElementById(`question-${firstErrorId}`)) {
      document.getElementById(`question-${firstErrorId}`)!.scrollIntoView({ behavior: 'smooth' });
    }

    return Object.keys(errors).length === 0;
  };

  const submitAnswers = async () => {
    if (!validateAnswers()) {
      showPopup('Заполните все поля', null);
      return;
    }

    setIsLoading(true);
    try {
      const sessionId = Cookies.get('anonymousSessionId');
      const token = localStorage.getItem('access_token');

      for (const q of questions) {
        const answer = answers[q.id];
        let payload: any = null;

        switch (q.questionTypeId) {
          case 1:
            payload = { AnswerText: String(answer).trim() };
            break;
          case 2:
            const singleOpt = q.options?.find(o => o.id === Number(answer));
            if (!singleOpt) continue;
            payload = { AnswerClose: singleOpt.order };
            break;
          case 3:
            const orders = (answer as number[])
              .map(id => q.options?.find(o => o.id === id)?.order)
              .filter(o => o !== undefined) as number[];
            if (orders.length === 0) continue;
            payload = { AnswerMultiple: orders };
            break;
          case 4:
            const scale = Number(answer);
            if (isNaN(scale)) continue;
            payload = { AnswerScale: scale };
            break;
          case 5:
            if (!answer) continue;
            payload = { AnswerClose: Number(answer) };
            break;
          default:
            continue;
        }

        await axios.post(
          `https://localhost:7109/questionnaire/access/${id}/questions/${q.id}/answer`,
          payload,
          {
            headers: {
              'Content-Type': 'application/json',
              'Authorization': `Bearer ${token}`,
              'X-Session-Id': sessionId || '',
            },
          }
        );
      }

      navigate('/Thanks');
    } catch (err: any) {
      console.error(err);
      setApiError('Ошибка отправки. Попробуйте позже.');
    } finally {
      setIsLoading(false);
    }
  };

  const showPopup = (text: string, event: React.MouseEvent | null) => {
    const xPos = event?.clientX || window.innerWidth / 2;
    const yPos = event?.clientY || window.innerHeight / 2;
    setPopup({ visible: true, text, x: xPos, y: yPos });
    setTimeout(() => setPopup(prev => ({ ...prev, visible: false })), 1800);
  };

  const handleInputChange = (questionId: number, value: AnswerValue) => {
    setAnswers(prev => ({ ...prev, [questionId]: value }));
    if (validationErrors[questionId]) {
      setValidationErrors(prev => {
        const newErr = { ...prev };
        delete newErr[questionId];
        return newErr;
      });
    }
  };

  const handleCheckboxChange = (questionId: number, optionId: number) => {
    setAnswers(prev => {
      const current = (prev[questionId] as number[]) || [];
      return {
        ...prev,
        [questionId]: current.includes(optionId)
          ? current.filter(id => id !== optionId)
          : [...current, optionId]
      };
    });
  };

  return {
    // state
    author, ansTitle, questions, answers, isLoading, apiError, validationErrors, popup,
    // actions
    submitAnswers, handleInputChange, handleCheckboxChange, showPopup
  };
};
