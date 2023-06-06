import { Dropdown } from "primereact/dropdown";
import React from "react";
import * as StreamMasterApi from '../store/iptvApi';
import { useSessionStorage } from "primereact/hooks";
import { type SelectItem } from "primereact/selectitem";

const M3UFilesSelector = (props: M3UFilesSelectorProps) => {

  const [selectedM3UFile, setSelectedM3UFile] = useSessionStorage<StreamMasterApi.M3UFilesDto>({ id: 0, name: 'All' } as StreamMasterApi.M3UFilesDto, props.id + '-setSelectedM3UFile');

  const m3uFilesQuery = StreamMasterApi.useM3UFilesGetM3UFilesQuery();

  React.useMemo(() => {
    if (props.value && !selectedM3UFile) {
      setSelectedM3UFile(props.value);
    }

  }, [props.value, selectedM3UFile, setSelectedM3UFile]);

  const options = React.useMemo(() => {
    if (!m3uFilesQuery.data) return [];

    return m3uFilesQuery.data.map((cg) => {
      return { label: cg.name, value: cg };
    });

  }, [m3uFilesQuery.data]);

  const selectedTemplate = React.useCallback((option: SelectItem) => {
    if (option === null) {
      return;
    }

    return (
      <div className='flex h-full justify-content-start align-items-center p-0 m-0'>
        {option.label}
      </div>
    );
  }, []);


  return (
    <div className="iconSelector flex w-full justify-content-start align-items-center">
      <Dropdown
        className='iconSelector p-0 m-0 w-full z-5'
        onChange={(e) => {
          setSelectedM3UFile(e.value);
          props.onChange(e.value);
        }
        }
        options={options}
        placeholder="Play List"
        style={{
          ...{
            backgroundColor: 'var(--mask-bg)',
            overflow: 'hidden',
            textOverflow: 'ellipsis',
            whiteSpace: 'nowrap',
          },
        }}
        value={selectedM3UFile}
        valueTemplate={selectedTemplate}

      />
    </div>
  );
};

M3UFilesSelector.displayName = 'M3UFilesSelector';
M3UFilesSelector.defaultProps = {
};

type M3UFilesSelectorProps = {
  id: string;
  onChange: ((value: StreamMasterApi.M3UFilesDto) => void);
  value: StreamMasterApi.M3UFilesDto;
};

export default React.memo(M3UFilesSelector);
