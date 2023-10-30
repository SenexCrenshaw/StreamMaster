import { useEpgFilesGetPagedEpgFilesQuery, type M3UFileDto, type M3UFilesGetPagedM3UFilesApiArg } from '@lib/iptvApi';
import { Dropdown } from 'primereact/dropdown';
import { useLocalStorage } from 'primereact/hooks';
import { type SelectItem } from 'primereact/selectitem';
import { memo, useCallback, useEffect, useMemo } from 'react';

const M3UFilesSelector = (props: M3UFilesSelectorProperties) => {
  const [selectedM3UFile, setSelectedM3UFile] = useLocalStorage<M3UFileDto>({ id: 0, name: 'All' } as M3UFileDto, `${props.id}-setSelectedM3UFile`);

  const m3uFilesQuery = useEpgFilesGetPagedEpgFilesQuery({} as M3UFilesGetPagedM3UFilesApiArg);

  useEffect(() => {
    if (props.value && !selectedM3UFile) {
      setSelectedM3UFile(props.value);
    }
  }, [props.value, selectedM3UFile, setSelectedM3UFile]);

  const options = useMemo(() => {
    if (!m3uFilesQuery.data?.data) return [];

    return m3uFilesQuery.data.data.map((cg) => ({ label: cg.name, value: cg }));
  }, [m3uFilesQuery.data]);

  const selectedTemplate = useCallback((option: SelectItem) => {
    if (option === null) {
      return;
    }

    return <div className="flex h-full justify-content-start align-items-center p-0 m-0">{option.label}</div>;
  }, []);

  return (
    <div className="iconSelector flex w-full justify-content-start align-items-center">
      <Dropdown
        className="iconSelector p-0 m-0 w-full z-5"
        onChange={(e) => {
          setSelectedM3UFile(e.value);
          props.onChange(e.value);
        }}
        options={options}
        placeholder="Play List"
        style={{

          backgroundColor: 'var(--mask-bg)',
          overflow: 'hidden',
          textOverflow: 'ellipsis',
          whiteSpace: 'nowrap'

        }}
        value={selectedM3UFile}
        valueTemplate={selectedTemplate}
      />
    </div>
  );
};

M3UFilesSelector.displayName = 'M3UFilesSelector';

interface M3UFilesSelectorProperties {
  readonly id: string;
  readonly onChange: (value: M3UFileDto) => void;
  readonly value: M3UFileDto;
}

export default memo(M3UFilesSelector);
