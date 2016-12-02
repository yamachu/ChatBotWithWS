let style = {
  chatList :{
    "overflow-x": "hidden",
    height: "calc((100vh) - ((100 + 50)px))"
  },
  chatItem :{

  },
  chatForm :{
    height: '100px',
    width: '100%',
    bottom: '0px',
    position: 'fixed'
  },
  chatTextArea :{
    "min-height": '100px',
    width: '70%'
  },
  chatTextButton :{
    width: '30%',
    "padding-top": "85px"
  }
};

class WSChat extends React.Component {
  constructor(props) {
    super(props);
    this.state = {items: []};
  }
  
  updateItems(newItem) {
    let allItems = this.state.items.concat([newItem]);
    this.setState({items: allItems});
  }

  render() {
    return(
      <div>
      <ChatList items={this.state.items} />
      <WSFormHandler onRecieved={this.updateItems.bind(this)}/>
      </div>
    ); 
  }
}

class ChatList extends React.Component {
  render() {
    var createItem = function(chatItem) {
      return (
        <ChatItem>{chatItem}</ChatItem>
      );
    };

    return(
       <ul style={Object.assign({

       })}> {this.props.items.map(createItem)} </ul>
    );
  }
}

class ChatItem extends React.Component {
  render() {
    return(
      <li>{this.props.children.text}</li>
    );
  }
}

class WSFormHandler extends React.Component {
    constructor(props) {
      super(props);
      this.state = {
        // will support (window.location.protocol == 'http:' ? 'ws:' : 'wss:') 
        url: 'ws:' + window.location.href.slice(window.location.protocol.length),
        sendItem: ''
      };
    }

    componentDidMount() {
      // initialize websocket
      let websocket = new WebSocket(this.state.url);
      this.setState({websocket: websocket});

      // console.dir(websocket);
      websocket.onopen = () => {
        console.log('WebSocket onOpen');
      };

      websocket.onmessage = (msg) => {
        let resp = JSON.parse(msg.data);
        console.log(resp);
        this.props.onRecieved(resp);
        return;
      };

      websocket.onclose = () => {
        console.log('WebSocket onClose');
      }

      websocket.onerror = (err) => {
        console.log('WebSocket onError', err);
      }
    }

    componentWillUnmount() {
      let websocket = this.state.websocket;
      websocket.close();
    }

    handleSubmit(event) {
      event.preventDefault();

      let websocket = this.state.websocket;
      let pack = {
        "text": this.state.sendItem
      };
      websocket.send(JSON.stringify(pack));
      
      this.setState({sendItem: ''});
      this.refs.tarea.focus();
    }

    onTextChange(event) {
      this.setState({sendItem: event.target.value});
    }

    render() {
      return (
        <form style={style.chatForm} onSubmit={this.handleSubmit.bind(this)}>
          <textarea style={style.chatTextArea} ref="tarea" value={this.state.sendItem} onChange={this.onTextChange.bind(this)}></textarea>
          <button style={style.chatTextButton} type="submit">Submit</button>
        </form>
      );
    }
}

ReactDOM.render(
  <WSChat />,
  document.getElementById('chatContainer')
);