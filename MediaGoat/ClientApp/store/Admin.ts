import { Action, Reducer } from 'redux';
import { AppThunkAction } from './';
import { fetch, addTask } from 'domain-task';
import { Song } from "./Songs";

// -----------------
// STATE - This defines the type of data maintained in the Redux store.

export enum IndexState {
    None = 0,
    Running = 1,
    Success = 2,
    Failed = 3
};

export interface IndexResult {
    status: IndexState,
    message?: string,
    startTime?: Date,
    stopTime?: Date
}

export interface AdminState {
    timer?: number;
    indexResult: IndexResult;
}

// -----------------
// ACTIONS - These are serializable (hence replayable) descriptions of state transitions.
// They do not themselves have any side-effects; they just describe something that is going to happen.
// Use @typeName and isActionType for type detection that works even after serialization/deserialization.

interface StartIndexRunAction { type: 'StartIndexRunAction' }
interface StartGettingIndexState { type: 'StartGettingIndexState', timer: number }
interface ReceiveIndexState { type: "ReceiveIndexState", indexResult: IndexResult }
interface StopGettingIndexState { type: 'StopGettingIndexState' }

// Declare a 'discriminated union' type. This guarantees that all references to 'type' properties contain one of the
// declared type strings (and not any other arbitrary string).
type KnownAction = StartIndexRunAction | StartGettingIndexState | ReceiveIndexState | StopGettingIndexState;

// ----------------
// ACTION CREATORS - These are functions exposed to UI components that will trigger a state transition.
// They don't directly mutate state, but they can have external side-effects (such as loading data).

export const actionCreators = {
    startIndexRun: (): AppThunkAction<KnownAction> => (dispatch, getState) => {
        // Only load data if it's something we don't already have (and are not already loading)
        let fetchTask = fetch(`/api/Admin/StartIndexRun`, { method: "POST" });

        addTask(fetchTask); // Ensure server-side prerendering waits for this to complete
        dispatch({ type: 'StartIndexRunAction' });
    },
    startGettingIndexState: (): AppThunkAction<KnownAction> => (dispatch, getState) => {
        if (!getState().admin.timer) {
            let timer = window.setInterval(() => {
                let fetchTask = fetch("/api/Admin/CurrentIndexingResult")
                    .then(response => response.json() as IndexResult)
                    .then(data => {
                        let typedData = {
                            ...data,
                            startTime: data.startTime ? new Date(data.startTime) : null,
                            stopTime: data.stopTime ? new Date(data.stopTime) : null
                        }

                        dispatch(actionCreators.receiveIndexState(typedData));
                    });

                addTask(fetchTask);
            }, 1000);
            dispatch({ type: "StartGettingIndexState", timer: timer });
        }
    },
    receiveIndexState: (indexResult: IndexResult) => <ReceiveIndexState>{ type: "ReceiveIndexState", indexResult: indexResult },
    stopGettingIndexState: (): AppThunkAction<KnownAction> => (dispatch, getState) => {
        let timer = getState().admin.timer;
        if (timer) {
            clearInterval(timer);

            dispatch({ type: "StopGettingIndexState" });
        }
    },
};

// ----------------
// REDUCER - For a given state and action, returns the new state. To support time travel, this must not mutate the old state.

export const reducer: Reducer<AdminState> = (state: AdminState, action: KnownAction) => {
    switch (action.type) {
        case 'StartIndexRunAction':
            return state;
        case "StartGettingIndexState":
            return {
                ...state, timer: action.timer
            };
        case "ReceiveIndexState":
            return {
                ...state, indexResult: action.indexResult
            };
        case "StopGettingIndexState":
            return {
                ...state, timer: null
            };
        default:
            //The following line guarantees that every action in the KnownAction union has been covered by a case above
            const exhaustiveCheck: never = action;
    }

    // For unrecognized actions (or in cases where actions have no effect), must return the existing state
    //  (or default initial state if none was supplied)
    return state || { timer: null, indexResult: { status: IndexState.None } };
};
