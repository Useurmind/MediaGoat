import * as React from 'react';
import { Link, RouteComponentProps } from 'react-router-dom';
import { connect } from 'react-redux';
import { ApplicationState } from '../store';
import * as PlayerStore from '../store/Player';

interface PlayerUiState {
    showDetails: boolean
}

type PlayerProps =
    PlayerStore.PlayerState
    & typeof PlayerStore.actionCreators
    & RouteComponentProps<{}>;

class Player extends React.Component<PlayerProps, PlayerUiState> {
    constructor(props: PlayerProps) {
        super(props);

        this.state = {
            showDetails: false
        };
    }


    public render() {
        let currentsong = this.props.currentSong;
        let hasSong: boolean = currentsong != null;
        let currentKey = hasSong ? currentsong.guid : "";
        let isPlaying = this.props.isPlaying;
        let playButton = <button onClick={e => this.props.playSong(null)} disabled={!hasSong}>
            <i className="fa fa-play" aria-hidden="true"></i>
        </button>
        let pauseButton = <button onClick={e => this.props.pauseSong()}>
            <i className="fa fa-pause" aria-hidden="true"></i>
        </button>

        return <div>
            <audio controls autoPlay key={currentKey}>
                {hasSong &&
                    <source src={currentsong.url} type={currentsong.contentType} />
                }
            </audio>
        </div>;
    }
}



// Wire up the React component to the Redux store
export default connect(
    (state: ApplicationState) => state.player, // Selects which state properties are merged into the component's props
    PlayerStore.actionCreators                 // Selects which action creators are merged into the component's props
)(Player);