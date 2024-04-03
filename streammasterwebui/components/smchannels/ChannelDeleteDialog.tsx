import { SMChannelDto } from '@lib/apiDefs';
import { useQueryFilter } from '@lib/redux/slices/useQueryFilter';
import { useSelectAll } from '@lib/redux/slices/useSelectAll';
import { useSelectedItems } from '@lib/redux/slices/useSelectedItemsSlice';
import { DeleteSMChannels, DeleteSMChannelsFromParameters } from '@lib/smAPI/SMChannels/SMChannelsCommands';
import { memo, useMemo, useState } from 'react';
import InfoMessageOverLayDialog from '../InfoMessageOverLayDialog';
import OKButton from '../buttons/OKButton';
import XButton from '../buttons/XButton';

interface ChannelDeleteDialogProperties {
  readonly iconFilled?: boolean;
  readonly id: string;
  readonly onClose?: () => void;
  readonly skipOverLayer?: boolean;
  // readonly values?: SMChannelDto[];
  readonly value?: SMChannelDto;
}

const ChannelDeleteDialog = ({ iconFilled, id, onClose, skipOverLayer, value }: ChannelDeleteDialogProperties) => {
  const [showOverlay, setShowOverlay] = useState<boolean>(false);
  const [infoMessage, setInfoMessage] = useState('');

  const [block, setBlock] = useState<boolean>(false);

  const { selectSelectedItems, setSelectSelectedItems } = useSelectedItems<SMChannelDto>(id);

  const { selectAll } = useSelectAll(id);
  const { queryFilter } = useQueryFilter(id);

  const ReturnToParent = () => {
    setShowOverlay(false);
    setInfoMessage('');
    setBlock(false);
    setSelectSelectedItems([]);
    onClose?.();
  };

  const deleteVideoStream = async () => {
    setBlock(true);

    if (selectAll === true) {
      if (!queryFilter) {
        ReturnToParent();

        return;
      }
      DeleteSMChannelsFromParameters(queryFilter)
        // await videoStreamsDeleteAllVideoStreamsFromParametersMutation(toSendAll)
        .then(() => {
          setInfoMessage('Set Stream Visibility Successfully');
        })
        .catch((error) => {
          setInfoMessage(`Set Stream Visibility Error: ${error.message}`);
        });

      return;
    }

    if (value !== undefined) {
      DeleteSMChannels([value.id])
        .then(() => {
          setInfoMessage('Delete Stream Successful');
        })
        .catch((error) => {
          setInfoMessage(`Delete Stream Error: ${error.message}`);
        });

      return;
    }

    const max = 500;

    let count = 0;

    const promises = [];
    let toSend: number[] = [];

    const ids = selectSelectedItems?.map((a: SMChannelDto) => a.id) ?? [];
    if (ids.length === 0) {
      ReturnToParent();
    }

    while (count < ids.length) {
      toSend = count + max < ids.length ? ids.slice(count, count + max) : ids.slice(count, ids.length);

      count += max;
      promises.push(
        DeleteSMChannels(toSend)
          .then(() => {})
          .catch(() => {})
      );
    }

    const p = Promise.all(promises);

    await p
      .then(() => {
        setInfoMessage('Delete Streams Successful');
      })
      .catch((error) => {
        setInfoMessage(`Delete Streams Error: ${error.message}`);
      });
  };

  const isFirstDisabled = useMemo(() => {
    if (value !== undefined) {
      return false;
    }

    if (!selectSelectedItems || selectSelectedItems?.length === 0) {
      return true;
    }

    return !selectSelectedItems[0].isUserCreated;
  }, [selectSelectedItems, value]);

  const getTotalCount = useMemo(() => {
    const count = selectSelectedItems?.length ?? 0;

    return count;
  }, [selectSelectedItems?.length]);

  if (skipOverLayer) {
    return <XButton disabled={isFirstDisabled} iconFilled={false} onClick={async () => await deleteVideoStream()} tooltip="Delete User Created Stream" />;
  }

  return (
    <>
      <InfoMessageOverLayDialog
        blocked={block}
        header="Delete?"
        infoMessage={infoMessage}
        onClose={() => {
          ReturnToParent();
        }}
        overlayColSize={2}
        show={showOverlay}
      >
        <div className="m-0 p-0 w-full">
          {/* <div className="m-3 border-1 border-red-500 w-full"> */}
          <div className="card flex mt-3 flex-wrap gap-2 justify-content-center ">
            <OKButton onClick={async () => await deleteVideoStream()} />
          </div>
          {/* </div> */}
        </div>
      </InfoMessageOverLayDialog>

      <XButton
        disabled={(isFirstDisabled || getTotalCount === 0) && !selectAll}
        iconFilled={iconFilled}
        onClick={() => setShowOverlay(true)}
        tooltip="Delete User Created Stream"
      />
    </>
  );
};

ChannelDeleteDialog.displayName = 'ChannelDeleteDialog';

export default memo(ChannelDeleteDialog);
