import React, { FC, ChangeEvent, useState, useRef, useEffect } from 'react';
import { useParams, useLocation, useNavigate } from 'react-router-dom';

import { useSurvey} from './useSurvey';

import QuestionComponent, { QuestionType } from './Question';
import './createStyle.css';
import ButtonMenuComponent from '../../component/ButtonMenu/ButtonMenuComponent';

import ModalLink from '../../component/modal/modalLinik/modalLink';

import OpenQuest from '../../img/SurveyPage/OpenQuestion.png';
import CloseQuest from '../../img/SurveyPage/CloseQuestion.png';
import MultiQuest from '../../img/SurveyPage/MultiQuestion.png';
import ShkalaQuest from '../../img/SurveyPage/ShkalaQuestion.png';
import InfoOutlined from '../../img/SurveyPage/InfoOutlined.png';
import TickIcon from '../../img/SurveyPage/TickIcon.png';


const SurveyPage: FC = () => {
    const { surveyId } = useParams<{ surveyId: string }>();
    const {
        isEditMode,
        title,
        setTitle,
        questions,
        isLoading,
        error,
        questionErrors,
        deleteError,
        dropdownsOpen,
        setDropdownsOpen,
        getQuestionIdentifier,
        getAnswerIdentifier,
        handleSaveOrUpdate,
        addNewQuestion,
        deleteQuestion,
        handleQuestionTextChange,
        handleOptionSelect,
        addAnswer,
        deleteAnswer,
        handleAnswerChange,
        handleScaleChange,
        setQuestionRef,
        isModalOpen,
        confirmSave,
        cancelSave,
        moveQuestion,
        handleDragStart,
        handleDragOver,
        handleDrop,
        handleDragEnd,
        handleDragLeave,
        setIsPublished,
        isPublished,
        handleTogglePublish,

    } = useSurvey(surveyId);

    interface LocationState {
        link: string;
    }

    const notDeletedQuestions = questions.filter(q => !q.isDeleting);
    const [createType, setCreateType] = useState('anketa');
    const [isModalOpenLink, setIsModalOpenLink] = useState<boolean>(false);
    const location = useLocation();
    const stateLink = (location.state as LocationState | null)?.link;
    const [publishedLink, setPublishedLink] = useState<string | null>(null);
    
    const navigate = useNavigate();

    // Получаем ID последнего вопроса
    const lastQuestionId = notDeletedQuestions.length > 0
        ? getQuestionIdentifier(notDeletedQuestions[notDeletedQuestions.length - 1])
        : 'placeholder';

    function linkModal() {
        setIsModalOpenLink(true);
    }
    function onCloseModalLink() {
        
        navigate('/account')
        setIsModalOpenLink(false)
    }

    return (
        <div className="survey-page-vh">
            <div className="survey-page">

                {/* Верхнее меню */}
                <ButtonMenuComponent
                    createType={createType}
                    setCreateType={setCreateType}
                    isLoading={isLoading}
                    publishedLink={publishedLink}
                    linkModal={linkModal}
                    disabled={true}
                    showButton={false}
                    isPublished={isPublished}
                    onTogglePublish={handleTogglePublish}
                />
                <form>
                    
                    {createType === 'anketa' && (
                        <div className="survey-title">
                            <input
                                type="text"
                                placeholder={error ? `${error}` : "Введите название"}
                                className={error ? 'error-message-create' : ''}
                                value={title}
                                onChange={(e: ChangeEvent<HTMLInputElement>) => setTitle(e.target.value)}

                                maxLength={250}
                                aria-label="Название анкеты"
                            />
                        </div>
                    )}

                    {createType === 'anketa' && questions.map((question) => (
                        <QuestionComponent
                            key={question.uniqueId}
                            question={question}
                            questionErrors={questionErrors}
                            deleteError={deleteError}
                            dropdownsOpen={dropdownsOpen}
                            onDropdownToggle={(id) => setDropdownsOpen(prev => ({ ...prev, [id]: !prev[id] }))}
                            onOptionSelect={handleOptionSelect}
                            onTextChange={handleQuestionTextChange}
                            onAnswerChange={handleAnswerChange}
                            onAddAnswer={addAnswer}
                            onDeleteAnswer={deleteAnswer}
                            onScaleChange={handleScaleChange}
                            onAddNew={addNewQuestion}
                            onMove={moveQuestion}
                            onDelete={deleteQuestion}
                            onDragStart={handleDragStart}
                            onDragOver={handleDragOver}
                            onDrop={handleDrop}
                            onDragEnd={handleDragEnd}
                            onDragLeave={handleDragLeave}
                            setQuestionRef={setQuestionRef}
                            isOnlyQuestion={notDeletedQuestions.length <= 0}
                            isLastQuestion={question.displayId === notDeletedQuestions.length}
                            canAddMoreQuestions={notDeletedQuestions.length < 10}
                        />
                    ))}

                    {notDeletedQuestions.length < 10 && createType === 'anketa' && (
                        <div className="add-question-placeholder">
                            <span className="add-question-title">Добавить новый вопрос</span>
                            <div className="question-type-buttons">
                                <div className="q-type-btn" onClick={() => addNewQuestion(lastQuestionId, 'Открытый')}>
                                    <span><img src={OpenQuest} alt="icons-open-question" className="q-type-icon-box" /></span>
                                    <span className="q-type-label">Открытый</span>
                                    <span className='tooltip'>
                                        <button className='tooltip-toggle' type='button'>
                                            <img src={InfoOutlined} alt="icons-info" className="q-type-info" />
                                        </button>
                                        <span className='tooltip-text'>Если нужен ответ <br /> в свободной форме</span>
                                    </span>
                                </div>
                                <div className="q-type-btn" onClick={() => addNewQuestion(lastQuestionId, 'Закрытый')}>
                                    <span><img src={CloseQuest} alt="icons-close-question" className="q-type-icon-box" /></span>
                                    <span className="q-type-label">Закрытый</span>
                                    <span className='tooltip'>
                                        <button className='tooltip-toggle' type='button'>
                                            <img src={InfoOutlined} alt="icons-info" className="q-type-info" />
                                        </button>
                                        <span className='tooltip-text'>Если нужно выбрать <br /> один вариант ответа</span>
                                    </span>
                                </div>
                                <div className="q-type-btn" onClick={() => addNewQuestion(lastQuestionId, 'Множественный выбор')}>
                                    <span><img src={MultiQuest} alt="icons-multi-question" className="q-type-icon-box" /></span>
                                    <span className="q-type-label">Несколько</span>
                                    <span className='tooltip'>
                                        <button className='tooltip-toggle' type='button'>
                                            <img src={InfoOutlined} alt="icons-info" className="q-type-info" />
                                        </button>
                                        <span className='tooltip-text'>Если нужно выбрать <br /> один или несколько <br /> вариантов ответа</span>
                                    </span>
                                </div>
                                <div className="q-type-btn" onClick={() => addNewQuestion(lastQuestionId, 'Шкала')}>
                                    <span><img src={ShkalaQuest} alt="icons-shkala-question" className="q-type-icon-box" /></span>
                                    <span className="q-type-label">Шкала</span>
                                    <span className='tooltip'>
                                        <button className='tooltip-toggle' type='button'>
                                            <img src={InfoOutlined} alt="icons-info" className="q-type-info" />
                                        </button>
                                        <span className='tooltip-text'>Если нужно оценить <br /> высказывание по шкале</span>
                                    </span>
                                </div>
                            </div>
                        </div>

                    )}
                    {createType === 'anketa' && (
                        <div className="ButtonSaveContainer">
                            <button
                                onClick={handleSaveOrUpdate}
                                className="ButtonSave"
                                type="button"
                                disabled={isLoading}
                            >
                                <img src={TickIcon} alt="icons-tick-question" className="TickIcon" />
                                {isLoading ? 'Отправка...' : 'СОХРАНИТЬ'}
                            </button>
                        </div>
                    )}
                </form>

                {
                    isModalOpen && (
                        <div className='modal'>
                            <div className='modal-content'>
                                <div className='modal-text'>
                                    <div style={{ fontSize: '20px', marginBottom: '20px' }}>Анкета создана!<br /> </div>
                                    Чтобы анкета стала доступной для <br />
                                    прохождения, её необходимо опубликовать
                                </div>
                                <div className='button-modal'>
                                    <button
                                        className='notPublishButton'
                                        onClick={async () => {
                                            const result = await confirmSave(false);
                                            if (result) {
                                                setPublishedLink(result.link);
                                                setIsPublished(false);
                                            }
                                            cancelSave();
                                        }}
                                    >
                                        Не публиковать
                                    </button>
                                    <button
                                        className='PublishButton'
                                        onClick={async () => {
                                            const result = await confirmSave(true);
                                            if (result) {
                                                setPublishedLink(result.link);
                                                setIsModalOpenLink(true);
                                            }
                                        }}
                                    >
                                        Опубликовать
                                    </button>
                                </div>
                            </div>
                        </div>
                    )
                }
                {
                    isModalOpenLink && (
                        <ModalLink
                            isOpen={isModalOpenLink}
                            onClose={() => onCloseModalLink()}
                            link={publishedLink || stateLink || 'https://ссылкиНет.ru'}
                        />
                    )
                }
            </div >
        </div >
    );
};





.survey-page-vh {
    min-height: 92.7vh;
    background-color: rgba(242, 242, 242, 1);
}

.survey-page {
    display: flex;
    flex-direction: column;
    align-items: center;
    min-height: 100%;
    padding: 20px;
    box-sizing: border-box;
}

.survey-form {
    width: 100%;
    display: flex;
    flex-direction: column;
    align-items: center;
}

.survey-content-wrapper {
    width: 100%;
    display: flex;
    flex-direction: column;
    align-items: center;
}

.survey-title,
.question-container,
.add-question-placeholder {
    width: 92%;
    max-width: 1030px;
    margin-bottom: 30px;
    box-sizing: border-box;
}

.survey-title {
    font-family: Geologica;
    font-weight: 500;
    padding: 20px;
    background: rgba(242, 242, 242, 1);
    border-radius: 10px;
    font-size: clamp(0px, 1vw, 28px);
    transition: all 0.3s ease-in-out;
}

.survey-title span {
    display: block;
    text-align: left;
    margin-bottom: 10px;
}

.survey-title input {
    background: rgba(242, 242, 242, 1);
    font-family: Geologica;
    font-weight: 500;
    border: none;
    height: 10px;
    width: 100%;
    font-size: 24px;
    padding: 5px 0;
    outline: none;
    transition: border-bottom 0.3s ease;
}

/* Placeholder */
.add-question-placeholder {
    background: rgba(242, 242, 242, 1);
    padding: 30px 40px;
    border: 2px dashed #000000;
    border-radius: 10px;
    text-align: center;
    opacity: 0;
    transform: translateY(20px);
    animation: fadeInUp 0.3s 0.3s ease-out forwards;
}

.add-question-title {
    font-family: Geologica;
    font-weight: 400;
    font-size: clamp(16px, 1vw, 20px);
    color: #505050;
}

.question-type-buttons {
    padding-top: 30px;
    display: grid;
    grid-template-columns: repeat(4, 1fr);
    gap: 12px;
}

.q-type-btn {
    display: flex;
    max-height: 39px;
    background-color: #fff;
    border: 1px solid #e0e0e0;
    border-radius: 5px;
    padding: 0px;
    cursor: pointer;
    transition: all 0.2s ease-in-out;
    box-shadow: 0 2px 4px rgba(0, 0, 0, 0.05);
    justify-content: space-between;
    width: 100%;
}

.q-type-btn:hover {
    transform: translateY(-2px);
    box-shadow: 0 4px 8px rgba(0, 0, 0, 0.1);
    border-color: #c0c9e0;
}

.q-type-icon-box {
    max-width: 40px;
    max-height: 40px;
}

.q-type-label {
    font-size: 16px;
    color: #333;
    margin: 1vh;
}

.tooltip-toggle {
    border: none;
    background-color: #ffffff;
    padding: 0;
    margin: 0;
    display: block;
    margin-top: 3px;
}

.q-type-info {
    font-size: 16px;
    color: #aaa;
    margin-top: 4px;
    margin-left: auto;
}

.q-type-info:hover {
    border-radius: 10px;
    background-color: #d0cece;
}

.tooltip-text {
    font-family: Geologica;
    background-color: #333333;
    color: #FFFFFF;
    font-size: 12px;
    line-height: 20px;
    font-weight: 300;
    text-transform: none;
    padding: 20px 18px 18px 22px;
    border-radius: 10px;
    width: 150px;
    position: absolute;
    top: 120%;
    left: 50%;
    z-index: 1;
    transform: translateX(-50%);
    display: none;
}

.tooltip-toggle:hover + .tooltip-text,
.tooltip-toggle:focus + .tooltip-text {
    display: block;
}

/* Контейнер вопросов */
.question-container {
    position: relative;
    overflow: hidden;
    max-height: 100%;
    transform: translateZ(0);
    border-top: 3px solid transparent;
}

.question-container.question-enter-active {
    opacity: 1;
    transform: scale(1) translateY(0);
    max-height: 1000px;
    padding-top: 20px;
    padding-bottom: 20px;
}

.question-container.question-exit-active {
    opacity: 0;
    transform: scale(0.9) translateX(30px);
    max-height: 0 !important;
    margin-bottom: 0 !important;
    padding-top: 0 !important;
    padding-bottom: 0 !important;
    border: 0;
    overflow: hidden;
}

.question-container.levitate-up,
.question-container.levitate-down {
    z-index: 5;
    box-shadow: 0 6px 20px rgba(0, 0, 0, 0.15);
}

.question-container.levitate-up {
    transform: scale(1.03) translateY(-10px);
}

.question-container.levitate-down {
    transform: scale(1.03) translateY(10px);
}

.question-container.dragging {
    opacity: 0.6;
    background: #f0f3ff;
    box-shadow: 0 8px 25px rgba(0, 0, 0, 0.2);
    transform: scale(1.02);
    z-index: 10;
    cursor: grabbing;
    border-top-color: transparent !important;
}

.question-container.make-space-down,
.question-container.make-space-up {
    border-top: 3px dashed #3f65f1;
    transition: padding-top 0.2s ease-out, border-top 0.2s ease-out;
}

.question-container.make-space-down {
    padding-top: 15px;
}

.question-container.make-space-up {
    padding-bottom: 15px;
}

/* Контейнер вопроса */
.question {
    width: 100%;
    padding: 20px;
    padding-bottom: 15px;
    background: rgb(255, 255, 255);
    border-radius: 10px;
    font-size: 28px;
    position: relative;
    border: 1px solid #e0e0e0;
    opacity: 0;
    transform: translateY(20px);
    animation: fadeInUp 0.1s 0.1s ease-out forwards;
    box-sizing: border-box;
}

.Type_question {
    font-family: Geologica;
    font-weight: 500;
    margin-right: auto;
    font-size: 0.93rem;
    margin-top: 6px;
}

.IconType-question {
    width: 35px;
    height: 35px;
}

.question span {
    display: block;
    text-align: left;
    margin-bottom: 10px;
}

.question input[type="text"],
.question input[type="number"] {
    font-size: 16px;
    padding: 10px 15px;
    outline: none;
    transition: border-bottom 0.3s ease;
    background-color: transparent;
    width: 100%;
    box-sizing: border-box;
}

.question input[type="number"] {
    width: 60px;
    text-align: center;
    padding: 3px;
    border: 1px solid #ccc;
    border-radius: 4px;
}

.question input:focus {
    border-bottom: 1px solid blue;
}

.error-message-create::placeholder {
    color: rgba(211, 47, 47, 1);
}

.newBlock,
.swap button,
.trash {
    font-family: Montserrat, sans-serif;
    font-size: 18px;
    color: rgba(80, 80, 80, 1);
    cursor: pointer;
    transition: color 0.2s ease-in-out, transform 0.15s ease;
    display: inline-flex;
    align-items: center;
    gap: 8px;
    background: none;
    border: none;
    padding: 0;
}

.newBlock:hover,
.swap button:not(:disabled):hover,
.trash:not(:disabled):hover {
    color: rgba(0, 0, 0, 1);
    transform: translateY(-1px);
}

.trash:hover:not(:disabled) {
    color: #d9534f;
}

.swap button:disabled,
.trash:disabled {
    cursor: not-allowed;
    opacity: 0.4;
}

.swap {
    display: flex;
    justify-content: flex-end;
    font-family: Montserrat, sans-serif;
    font-size: 18px;
    margin-left: 10px;
    color: rgba(80, 80, 80, 1);
    gap: 8px;
}

.swap svg {
    width: 24px;
    height: 24px;
    fill: currentColor;
}

.trash {
    margin-left: auto;
}

.trash .bi-trash {
    width: 24px;
    height: 24px;
}

.add-button {
    font-family: Montserrat, sans-serif;
    font-size: 13px;
    color: rgba(0, 71, 70, 1);
    padding: 8px 15px;
    margin-top: 20px;
    border-radius: 6px;
    border: none;
    background: rgb(255, 255, 255);
    border: none;
    cursor: pointer;
    transition: background-color 0.2s ease, transform 0.15s ease;
    display: flex;
    justify-content: center;
    align-items: center;
}

.plus {
    width: 20px;
}

.add-button:hover:not(:disabled) {
    background-color: rgb(255, 255, 255);
    transform: scale(1.05);
}

.add-button:disabled {
    background-color: #f8c98a;
    cursor: not-allowed;
}

.delete-button {
    font-family: Montserrat, sans-serif;
    font-size: 20px;
    line-height: 1;
    color: rgba(136, 133, 131, 1);
    padding: 0 10px;
    border-radius: 50%;
    background: #ffffff;
    border: 1px solid #ffffff;
    cursor: pointer;
    transition: all 0.2s ease;
    width: 28px;
    height: 28px;
    display: inline-flex;
    align-items: center;
    justify-content: center;
}

.delete-button:hover {
    background-color: #ff6b6b;
    color: white;
    border-color: #ff4d4d;
    transform: scale(1.1) rotate(90deg);
}

.answer-container {
    margin-top: 5px;
    display: flex;
    align-items: center;
    gap: 10px;
    width: 100%;
    transition: opacity 0.3s ease-in-out, transform 0.3s ease-in-out;
}

.answer-container input[type="text"] {
    flex: 1;
}

.answer-container .delete-button {
    flex-shrink: 0;
    margin-top: 0;
}

.ButtonSaveContainer {
    display: flex;
    justify-content: center;
    align-items: center;
    gap: 16px;
    width: 92%;
    max-width: 1030px;
    margin: 0 auto;
    padding: 0 16px;
    position: relative;
    margin-top: 60px;
    opacity: 0;
    transform: translateY(20px);
    animation: fadeInUp 0.3s 0.3s ease-out forwards;
}

.ButtonSave {
    font-family: Geologica;
    background: rgba(0, 71, 70, 1);
    border-radius: 3px;
    font-weight: 300;
    font-size: 0.9375rem;
    color: rgba(255, 255, 255, 0.95);
    border: none;
    cursor: pointer;
    transition: background-color 0.2s ease, transform 0.2s ease, box-shadow 0.2s ease;
    display: flex;
    justify-content: center;
    text-align: center;
    margin-left: auto;
}

.ButtonSave:hover:not(:disabled) {
    background-color: rgba(0, 71, 70, 1);
    transform: translateY(-2px);
    box-shadow: 0 4px 10px rgba(0, 71, 70, 1);
}

.ButtonSave:disabled {
    cursor: not-allowed;
    background: rgba(204, 202, 201, 1);
}

.error-message-create {
    color: #e53935;
    font-size: 14px;
    margin-top: 8px;
    font-weight: 500;
    animation: shake 0.4s ease-in-out;
}

.Error-transfer {
    display: block;
}

.notPublishButton {
    background-color: white;
}

.notPublishButton:hover {
    color: #ff0000;
    border-color: white;
}

.PublishButton {
    border-radius: 3px;
    background: rgba(0, 71, 70, 1);
    color: white;
}

.PublishButton:hover {
    background-color: rgba(0, 71, 70, 1);
    transform: translateY(-2px);
    box-shadow: 0 4px 10px rgba(0, 71, 70, 1);
    border-color: rgba(0, 71, 70, 1);
}

hr {
    background-color: rgba(204, 202, 201, 1);
    border: 0;
    height: 1px;
    margin: 22px 0 22px 0;
    box-sizing: content-box;
    max-width: 400px;
    width: 100%;
}

.icon-button {
    width: 40px;
    height: 40px;
    cursor: pointer;
    border-radius: 4px;
    padding: 8px;
    transition: background-color 0.3s ease, transform 0.2s ease;
    box-sizing: border-box;
    margin-left: auto;
    border: 1px solid rgba(0, 71, 70, 1)
}

.icon-button img {
    max-width: 100%;
    max-height: 100%;
}

.icon-button:hover {
    background-color: rgba(0, 0, 0, 0.1);
    transform: scale(1.1);
}

@keyframes fadeInUp {
    from {
        opacity: 0;
        transform: translateY(20px);
    }
    to {
        opacity: 1;
        transform: translateY(0);
    }
}

@keyframes shake {
    0%, 100% {
        transform: translateX(0);
    }
    25% {
        transform: translateX(-5px);
    }
    50% {
        transform: translateX(5px);
    }
    75% {
        transform: translateX(-5px);
    }
}

@media (max-width: 1100px) {
    .question-type-buttons {
        grid-template-columns: repeat(2, 1fr);
    }
}

@media (max-width: 830px) {
    .question-type-buttons {
        grid-template-columns: repeat(2, 1fr);
    }
}

@media (max-width: 767px) {
    .Type-Switcher {
        width: 100%;
        justify-content: center;
    }
    
    .Type-Switcher button {
        flex: 1;
        min-width: 120px;
    }
    
    .ButtonSave {
        width: 50%;
    }
    
    .survey-title,
    .question-container,
    .add-question-placeholder {
        width: 95%;
    }
}

@media (max-width: 640px) {
    .question-type-buttons {
        grid-template-columns: repeat(1, 1fr);
    }
    
    .survey-title,
    .question-container,
    .add-question-placeholder {
        width: 98%;
        padding: 15px;
    }
}

@media (max-width: 500px) {
    .survey-title {
        margin: 20px 0px 0px 0px;
    }
    
    .ButtonSave {
        margin-right: 30px;
    }
    
    .question input[type="text"] {
        width: 100%;
    }
}

@media (max-width: 400px) {
    .survey-page {
        margin-left: 0;
        padding: 10px;
    }
    
    .survey-title,
    .question-container,
    .add-question-placeholder {
        width: 100%;
        padding: 10px;
    }
    
    .ButtonSave {
        margin-right: 0;
        width: 100%;
    }
}

export default SurveyPage;
