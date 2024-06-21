import SMPopUp from '@components/sm/SMPopUp';
import CreateSMChannelsFromSMStreamsDialog from '@components/smchannels/CreateSMChannelsFromSMStreamsDialog';
import CreateSMStreamDialog from '@components/smstreams/CreateSMStreamDialog';
import StreamMultiVisibleDialog from '@components/smstreams/StreamMultiVisibleDialog';
import { memo } from 'react';

export interface SChannelMenuProperties {}

const SMStreamMenu = () => {
  // const items: MenuItem[] = [
  //   {
  //     className: 'icon-yellow',
  //     command: () => {},
  //     icon: 'pi pi-plus-circle',
  //     label: 'Add Selected'
  //   },
  //   {
  //     label: 'Update',
  //     icon: 'pi pi-refresh',
  //     command: () => {}
  //   },
  //   {
  //     label: 'Delete',
  //     icon: 'pi pi-trash',
  //     command: () => {}
  //   },
  //   {
  //     label: 'Upload',
  //     icon: 'pi pi-upload',
  //     command: () => {}
  //   },
  //   {
  //     label: 'React Website',
  //     icon: 'pi pi-external-link',
  //     command: () => {
  //       window.location.href = 'https://react.dev/';
  //     }
  //   }
  // ];

  // return (
  //   // <SMOverlay placement="bottom" icon="pi-bars" iconFilled buttonClassName="icon-orange" contentWidthSize="11rem">
  //   //   <div className="sm-channel-menu gap-2">
  //   //     <CreateSMChannelsFromSMStreamsDialog
  //   //       label="Stream to Channels"
  //   //       selectedItemsKey="selectSelectedSMStreamDtoItems"
  //   //       id="streameditor-SMStreamDataSelector"
  //   //     />
  //   //     <StreamMultiVisibleDialog label="Set Visibility" selectedItemsKey="selectSelectedSMStreamDtoItems" id="streameditor-SMStreamDataSelector" />
  //   //     <CreateSMStreamDialog label="Create Stream" />
  //   //   </div>
  //   // </SMOverlay>
  //   <div className="pl-3 z-5">
  //     <Tooltip target=".speeddial-bottom-left .p-speeddial-action" position="left" />
  //     <SpeedDial
  //       className="speeddial-bottom-left"
  //       model={items}
  //       direction="down"
  //       buttonClassName="sm-button icon-orange-filled"
  //       showIcon="pi pi-bars"
  //       style={{ right: '-13px', top: '46px' }}
  //     />
  //   </div>
  // <SMButton
  //   icon="pi-bars"
  //   iconFilled
  //   buttonTemplate={<SpeedDial className="top-50" model={items} direction="down" buttonClassName="sm-button icon-red-filled" />}
  // ></SMButton>
  // <SMPopUp
  //   placement="bottom"
  //   icon="pi-bars"
  //   iconFilled
  //   // buttonClassName="icon-orange"
  //   contentWidthSize="11rem"
  //   buttonTemplate={<SpeedDial className="to-50" model={items} direction="down" buttonClassName="sm-button icon-red-filled" />}
  // >
  //   {/* <div className="sm-channel-menu gap-2">
  //     <CreateSMChannelsFromSMStreamsDialog selectedItemsKey="selectSelectedSMStreamDtoItems" id="streameditor-SMStreamDataSelector" />
  //     <CreateSMStreamDialog label="Add Custom" />
  //     <StreamMultiVisibleDialog label="Hide Selected" selectedItemsKey="selectSelectedSMStreamDtoItems" id="streameditor-SMStreamDataSelector" />
  //   </div> */}
  // </SMPopUp>
  // <div>
  //   <SpeedDial className="to-50" model={items} direction="down" buttonClassName="sm-button icon-red-filled" />
  // </div>
  return (
    <SMPopUp placement="bottom" icon="pi-bars" iconFilled buttonClassName="icon-orange" contentWidthSize="11rem">
      <div className="sm-channel-menu gap-2">
        <CreateSMChannelsFromSMStreamsDialog selectedItemsKey="selectSelectedSMStreamDtoItems" id="streameditor-SMStreamDataSelector" />
        <CreateSMStreamDialog label="Add Custom" />
        <StreamMultiVisibleDialog label="Hide Selected" selectedItemsKey="selectSelectedSMStreamDtoItems" id="streameditor-SMStreamDataSelector" />
      </div>
    </SMPopUp>
  );
};

SMStreamMenu.displayName = 'SMStreamMenu';

export default memo(SMStreamMenu);
