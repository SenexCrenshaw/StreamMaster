import { useQueryFilter } from '@lib/redux/slices/useQueryFilter';
import { useSelectAll } from '@lib/redux/slices/useSelectAll';
import { useSelectedVideoStreams } from '@lib/redux/slices/useSelectedVideoStreams';

import { memo, useCallback, useEffect, useMemo, useState } from 'react';
import InfoMessageOverLayDialog from '../InfoMessageOverLayDialog';
import VisibleButton from '../buttons/VisibleButton';
import { SMStreamDto, ToggleSMStreamVisibleByIdRequest } from '@lib/smAPI/smapiTypes';
import { ToggleSMStreamVisibleById } from '@lib/smAPI/SMStreams/SMStreamsCommands';

interface StreamVisibleDialogProperties {
  readonly iconFilled?: boolean;
  readonly id: string;
  readonly onClose?: () => void;
  readonly skipOverLayer?: boolean;
  readonly values?: SMStreamDto[];
}

const StreamVisibleDialog = ({ id, iconFilled, onClose, skipOverLayer, values }: StreamVisibleDialogProperties) => {
  const [showOverlay, setShowOverlay] = useState<boolean>(false);
  const [block, setBlock] = useState<boolean>(false);
  const [infoMessage, setInfoMessage] = useState('');
  const [selectStreamsInternal, setSelectStreamsInternal] = useState<SMStreamDto[] | undefined>();

  const { selectedVideoStreams } = useSelectedVideoStreams(id);
  const { selectAll } = useSelectAll(id);
  const { queryFilter } = useQueryFilter(id);

  const ReturnToParent = useCallback(() => {
    setShowOverlay(false);
    setInfoMessage('');
    setBlock(false);
    onClose?.();
  }, [onClose]);

  useEffect(() => {
    if (values) {
      setSelectStreamsInternal(values);
    }
  }, [values]);

  useEffect(() => {
    if (!values) {
      setSelectStreamsInternal(selectedVideoStreams);
    }
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [selectedVideoStreams]);

  const getTotalCount = useMemo(() => selectStreamsInternal?.length ?? 0, [selectStreamsInternal]);

  const onVisiblesClick = useCallback(async () => {
    if (selectStreamsInternal === undefined) {
      ReturnToParent();
      return;
    }

    setBlock(true);

    if (getTotalCount !== 1 && selectAll === true) {
      if (!queryFilter) {
        ReturnToParent();

        return;
      }

      const toSendAll = {} as VideoStreamsUpdateAllVideoStreamsFromParametersApiArg;

      toSendAll.parameters = queryFilter;
      // toSendAll.parameters.pageSize = getTotalCount;

      toSendAll.request = {
        toggleVisibility: true
      } as UpdateVideoStreamRequest;

      // await UpdateAllVideoStreamsFromParameters(toSendAll)
      //   .then(() => {
      //     setInfoMessage('Toggle Stream Visibility Successfully');
      //   })
      //   .catch((error) => {
      //     setInfoMessage(`Toggle Stream Visibility Error: ${error.message}`);
      //   });

      return;
    }

    if (selectStreamsInternal.length === 0) {
      ReturnToParent();

      return;
    }

    await ToggleSMStreamVisibleById({ id: selectStreamsInternal[0].id as string } as ToggleSMStreamVisibleByIdRequest)
      .then(() => {
        setInfoMessage('Set Stream Visibility Successfully');
      })
      .catch((error) => {
        setInfoMessage(`Set Stream Visibility Error: ${error.message}`);
      });
  }, [selectStreamsInternal, getTotalCount, selectAll, ReturnToParent, queryFilter]);

  if (skipOverLayer === true) {
    return (
      <VisibleButton
        disabled={!getTotalCount}
        iconFilled={false}
        label="Toggle Visibility"
        onClick={async () => await onVisiblesClick()}
        tooltip="Toggle Visibility"
      />
    );
  }

  return (
    <>
      <InfoMessageOverLayDialog
        blocked={block}
        closable
        header="Toggle Visibility?"
        infoMessage={infoMessage}
        onClose={() => {
          ReturnToParent();
        }}
        overlayColSize={2}
        show={showOverlay}
      >
        <div className="flex justify-content-center w-full align-items-center h-full">
          <VisibleButton disabled={getTotalCount === 0 && !selectAll} label="Toggle Visibility" onClick={async () => await onVisiblesClick()} />
        </div>
      </InfoMessageOverLayDialog>
      <VisibleButton disabled={getTotalCount === 0 && !selectAll} iconFilled={iconFilled} onClick={() => setShowOverlay(true)} tooltip="Toggle Visibility" />
    </>
  );
};

StreamVisibleDialog.displayName = 'StreamVisibleDialog';

export default memo(StreamVisibleDialog);
