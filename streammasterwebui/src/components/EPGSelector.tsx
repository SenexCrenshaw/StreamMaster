import * as React from 'react';
import { classNames } from 'primereact/utils';
import DropDownEditorBodyTemplate from './DropDownEditorBodyTemplate';
import {
  type UpdateVideoStreamRequest,
  type ProgrammeNameDto,
  type VideoStreamDto
} from '../store/iptvApi';
import {
  useProgrammesGetProgrammeNamesQuery,
  useVideoStreamsUpdateVideoStreamMutation
} from '../store/iptvApi';

const EPGSelector = ({
  className = '',
  data,
  enableEditMode = true,
  value,
  onChange
}: EPGSelectorProps) => {
  const [programme, setProgramme] = React.useState<string>('');
  const [dataDataSource, setDataSource] = React.useState<ProgrammeNameDto[]>([]);
  const programmeNamesQuery = useProgrammesGetProgrammeNamesQuery();
  const [videoStreamsUpdateVideoStreamMutation] = useVideoStreamsUpdateVideoStreamMutation();

  React.useEffect(() => {
    if (programmeNamesQuery.data) {
      const newData = [{ displayName: '<Blank>' }, ...programmeNamesQuery.data];
      setDataSource(newData);
    }
  }, [programmeNamesQuery.data]);

  React.useEffect(() => {
    if (!dataDataSource || value === null || value === undefined) {
      setProgramme('<Blank>');
      return;
    }

    const foundProgramme = programmeNamesQuery.data?.find((a) => a.channel === value);
    if (foundProgramme?.displayName) {
      setProgramme(foundProgramme.displayName);
    }
  }, [dataDataSource, programmeNamesQuery.data, value]);

  const onEPGChange = React.useCallback((displayName: string) => {
    if (!data?.id) {
      onChange?.(displayName);
      return;
    }

    const requestData: UpdateVideoStreamRequest = {
      id: data.id,
      tvg_ID: displayName
    };

    void videoStreamsUpdateVideoStreamMutation(requestData)
      .then(() => {
        onChange?.(displayName);
      })
      .catch((error) => {
        console.error('Error updating video stream:', error);
      });

  }, [data, onChange, videoStreamsUpdateVideoStreamMutation]);


  const computedClassName = classNames('iconSelector p-0 m-0 w-full z-5', className);

  if (!enableEditMode) {
    return (
      <div className='flex h-full justify-content-center align-items-center p-0 m-0'>
        {programme || 'Dummy'}
      </div>
    );
  }

  return (
    <div className="iconSelector flex w-full justify-content-center align-items-center">
      <DropDownEditorBodyTemplate
        className={computedClassName}
        data={dataDataSource.map(a => a.displayName ?? '')}
        onChange={onEPGChange}
        value={programme}
      />
    </div>
  );
};

EPGSelector.displayName = 'EPGSelector';

type EPGSelectorProps = {
  className?: string;
  data?: VideoStreamDto;
  enableEditMode?: boolean;
  onChange?: (value: string) => void;
  value?: string;
};

export default React.memo(EPGSelector);
