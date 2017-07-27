import { Action, Reducer } from 'redux';
import { AppThunkAction } from './';
import { fetch, addTask } from 'domain-task';
import * as Player from "./Player";

// -----------------
// STATE - This defines the type of data maintained in the Redux store.

export interface Song {
    guid: string;
    title: string;
    artist: string;
    album: string;
    url: string;
    albumArtUrl: string;
    contentType: string;
}

export interface SongsState {
    songs: Song[];
    isLoading: boolean;
    searchString: string;
    playedSong: Song;
}

// -----------------
// ACTIONS - These are serializable (hence replayable) descriptions of state transitions.
// They do not themselves have any side-effects; they just describe something that is going to happen.
// Use @typeName and isActionType for type detection that works even after serialization/deserialization.

interface SearchSongsAction { type: 'SearchSongsAction', searchString: string }
interface ReceiveSongsAction { type: 'ReceiveSongsAction', songs: Song[] }

// Declare a 'discriminated union' type. This guarantees that all references to 'type' properties contain one of the
// declared type strings (and not any other arbitrary string).
type KnownAction = SearchSongsAction | ReceiveSongsAction;

// ----------------
// ACTION CREATORS - These are functions exposed to UI components that will trigger a state transition.
// They don't directly mutate state, but they can have external side-effects (such as loading data).

export const actionCreators = {
    searchSongs: (searchString: string): AppThunkAction<KnownAction> => (dispatch, getState) => {
        // Only load data if it's something we don't already have (and are not already loading)
        let fetchTask = fetch(`/api/Song/Songs?searchString=${searchString}`)
            .then(response => response.json() as Promise<Song[]>)
            .then(data => {
                dispatch(actionCreators.receiveSongs(data));
            });

        addTask(fetchTask); // Ensure server-side prerendering waits for this to complete
        dispatch({ type: 'SearchSongsAction', searchString: searchString });
    },
    receiveSongs: (songs: Song[]) => <ReceiveSongsAction>{ type: 'ReceiveSongsAction', songs: songs }
};

// ----------------
// REDUCER - For a given state and action, returns the new state. To support time travel, this must not mutate the old state.

export const reducer: Reducer<SongsState> = (state: SongsState, action: KnownAction) => {
    switch (action.type) {
        case 'SearchSongsAction':
            return {
                songs: state.songs,
                isLoading: true,
                searchString: action.searchString,
                playedSong: state.playedSong
            };
        case 'ReceiveSongsAction':
            return {
                songs: action.songs,
                isLoading: false,
                searchString: state.searchString,
                playedSong: state.playedSong
            };
        default:
            // The following line guarantees that every action in the KnownAction union has been covered by a case above
            const exhaustiveCheck: never = action;
    }

    // For unrecognized actions (or in cases where actions have no effect), must return the existing state
    //  (or default initial state if none was supplied)
    return state || { songs: [], isLoading: false, searchString: "", playedSong: null };
};
