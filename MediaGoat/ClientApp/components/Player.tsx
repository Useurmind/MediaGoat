import * as React from 'react';
import { Link, RouteComponentProps } from 'react-router-dom';
import { connect } from 'react-redux';
import { ApplicationState } from '../store';
import * as PlayerStore from '../store/Player';
import { Song } from '../store/Songs';

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

    toggleSongDetails() {
        this.setState({ ...this.state, showDetails: !this.state.showDetails });
    }

    public render() {
        let currentsong = this.props.currentSong;
        let hasSong: boolean = currentsong != null;
        let currentKey = hasSong ? currentsong.guid : "";
        let isPlaying = this.props.isPlaying;

        return <div className="bottomPlayer container-fluid">
            <audio controls autoPlay key={currentKey} >
                {hasSong &&
                    <source src={currentsong.url} type={currentsong.contentType} />
                }
            </audio>
            {hasSong && <div className="text">{currentsong.artist} - {currentsong.title}</div>}
            {hasSong && <button title="Show more infos about song" onClick={e => this.toggleSongDetails()}>
                <i className="fa fa-bars" aria-hidden="true"></i>
            </button>
            }
            {this.state.showDetails && <PlayerDetailsPane song={currentsong} /> }
        </div>;
    }
}

interface PlayerDetailsPaneProps {
    song: Song;
}

class PlayerDetailsPane extends React.Component<PlayerDetailsPaneProps, {}> {
    constructor(props: PlayerDetailsPaneProps) {
        super(props);

        this.state = {
        };
    }

    render() {
        let currentsong = this.props.song;
        let hasSong: boolean = currentsong != null;
        let albumArtImage = hasSong && currentsong.albumArtUrl ? <img src={currentsong.albumArtUrl} alt="noalt" className="img-rounded" /> : <i className="fa fa-music fa-4" aria-hidden="true"></i>

        return <div className="bottomPlayer-detailsPane" >
            <div className="container-fluid">
                <div className="row">
                    <div className="col-sm-3">
                        {albumArtImage}
                    </div>
                </div>
                <div className="row">
                    <div className="col-sm-3">
                        <div className="form-group">
                            <label htmlFor="playerArtist">Artist</label>
                            <input type="text" disabled={true} className="form-control" id="playerArtist" value={currentsong.artist} />
                        </div>
                    </div>
                </div>
                <div className="row">
                    <div className="col-sm-3">
                        <div className="form-group">
                            <label htmlFor="playerTitle">Title</label>
                            <input type="text" disabled={true} className="form-control" id="playerTitle" value={currentsong.title} />
                        </div>
                    </div>
                </div>
                <div className="row">
                    <div className="col-sm-3">
                        <div className="form-group">
                            <label htmlFor="playerAlbum">Album</label>
                            <input type="text" disabled={true} className="form-control" id="playerAlbum" value={currentsong.album} />
                        </div>
                    </div>
                </div>
            </div>

        </div >
    }
}



// Wire up the React component to the Redux store
export default connect(
    (state: ApplicationState) => state.player, // Selects which state properties are merged into the component's props
    PlayerStore.actionCreators                 // Selects which action creators are merged into the component's props
)(Player);