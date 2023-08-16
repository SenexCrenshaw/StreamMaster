
import { type DropdownFilterEvent } from 'primereact/dropdown';
import { type DropdownChangeEvent } from 'primereact/dropdown';
import { Dropdown } from 'primereact/dropdown';
import { classNames } from 'primereact/utils';
import { type IconsGetIconsApiArg } from '../store/iptvApi';
import { useIconsGetIconsQuery } from '../store/iptvApi';
import { type IconsGetIconsSimpleQueryApiArg } from '../store/iptvApi';
import { type IconFileDto } from '../store/iptvApi';
import { useIconsGetIconsSimpleQueryQuery } from '../store/iptvApi';
import StreamMasterSetting from '../store/signlar/StreamMasterSetting';
import { areIconFileDtosEqual, type DataTableFilterMetaData } from '../common/common';
import { addOrUpdateValueForField, getIconUrl } from '../common/common';

import { type VirtualScrollerTemplateOptions } from 'primereact/virtualscroller';
import React, { useState, useEffect, useCallback, useMemo } from 'react';
import { Skeleton } from 'primereact/skeleton';
import { GetIconFromSource } from '../store/signlar_functions';


const IconSelector = (props: IconSelectorProps) => {
  const [selectedIcon, setSelectedIcon] = useState<string>('');
  const [filter, setFilter] = useState<string>('');
  const setting = StreamMasterSetting();

  const [index, setIndex] = useState<number>(0);


  const [first, setFirst] = useState<number>(0);
  const [last, setLast] = useState<number>(100);

  const [dataSource, setDataSource] = useState<IconFileDto[]>([]);
  const [oldDataSource, setOldDataSource] = useState<IconFileDto[]>([]);

  const filteredIcons = useIconsGetIconsQuery({ jsonFiltersString: filter, pageSize: 40 } as IconsGetIconsApiArg);
  const icons = useIconsGetIconsSimpleQueryQuery({ first: first, last: last === 0 ? 1 : last } as IconsGetIconsSimpleQueryApiArg);

  useEffect(() => {
    if (filter === undefined || filter === '') {
      if (oldDataSource.length > 0) {
        setDataSource([...oldDataSource]);
        setIndex(oldDataSource.length);
        setOldDataSource([]);
      }
    }

    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [filter]);

  useEffect(() => {
    if (filteredIcons.data?.data !== undefined && filteredIcons.data.data.length > 0) {
      setOldDataSource([...dataSource]);

      const newDataSource = [] as IconFileDto[];
      filteredIcons.data.data.forEach(function (cn) {
        const foundIndex = newDataSource.findIndex((x) => x.id === cn.id || x.source === cn.source);
        if (foundIndex === -1) {
          newDataSource.push(cn);
        }
      });

      setIndex(newDataSource.length);
      setDataSource(newDataSource);
    }
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [filteredIcons]);

  useEffect(() => {
    if (icons.data !== undefined && icons.data.length > 0) {
      if (!areIconFileDtosEqual(icons.data, dataSource)) {

        const newDataSource = [...dataSource];
        icons.data.forEach(function (cn) {
          const foundIndex = newDataSource.findIndex((x) => x.id === cn.id || x.source === cn.source);
          if (foundIndex === -1) {
            newDataSource.push(cn);
          }
        });
        setIndex(newDataSource.length);
        setDataSource(newDataSource);
      }
    }
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [icons.data]);

  useEffect(() => {
    if (props.value !== undefined) {
      console.debug(props.value)
      setSelectedIcon(props.value);
      GetIconFromSource(props.value).then
        (x => {
          if (x !== undefined) {
            const newDataSource = [...dataSource];
            const foundIndex = newDataSource.findIndex((item) => item.id === x.id);
            if (foundIndex === -1) {
              newDataSource.push(x);
            }

            setIndex(newDataSource.length);
            setDataSource(newDataSource);
          }
        }).catch(e => console.error(e));
    }
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [props.value]);


  const onChange = useCallback((event: DropdownChangeEvent) => {
    setSelectedIcon(event.value);

    if (!event.value) return;

    const iconSource = event.value as string;

    props.onChange?.(iconSource);

  }, [props]);

  const selectedTemplate = useMemo(() => {
    if (selectedIcon === undefined) {
      return (<div />);
    }

    if (props.useDefault !== true && selectedIcon === '') {
      return (<div />);
    }

    const iconUrl = getIconUrl(selectedIcon, setting.defaultIconUrl, false);
    return (
      <div className=' max-h-1rem justify-content-start align-items-center p-0 m-0 pl-2'>

        <img
          alt='Icon logo'
          src={iconUrl}
          style={{
            maxWidth: '1rem',
            objectFit: 'contain',

          }}
        />
      </div>
    );
  }, [props.useDefault, selectedIcon, setting.defaultIconUrl]);

  const iconOptionTemplate = useCallback((option: IconFileDto) => {

    const iconUrl = getIconUrl(option.source, setting.defaultIcon, false);

    return (
      <div
        className="flex col-6 w-full align-items-center text-base text-color surface-overlay appearance-none outline-none focus:border-primary">
        <img
          alt={option.name ?? 'name'}
          className="surface-overlay appearance-none outline-none focus:border-primary"
          src={iconUrl}
          style={{
            maxHeight: '78px',
            maxWidth: '4rem',
            objectFit: 'contain',
            width: '4rem'
          }}
        />
        <div className="white-space-normal w-full col-6">{option.name}</div>
      </div>
    );
  }, [setting]);


  const className = classNames('iconSelector align-contents-center p-0 m-0 max-w-full w-full z-5 ', props.className, {
    'p-button-loading': props.loading,
    'p-disabled': props.disabled,
  });


  if (props.enableEditMode !== true) {
    const iconUrl = getIconUrl(selectedIcon, setting.defaultIconUrl, false);

    return (
      <img
        alt='logo'
        className="max-h-1rem"
        src={iconUrl}
        style={{
          maxWidth: '1.5rem',
          objectFit: 'contain',
        }}
      />
    )
  }

  const loadingTemplate = (options: VirtualScrollerTemplateOptions) => {
    const className2 = classNames('flex align-items-center p-2', {
      odd: options.odd
    });

    return (
      <div className={className2} style={{ height: '50px' }}>
        <Skeleton height="1.3rem" width={options.even ? '60%' : '50%'} />
      </div>
    );
  };

  const onFilter = (event: DropdownFilterEvent) => {
    console.debug(event);
    const tosend = [] as DataTableFilterMetaData[];
    addOrUpdateValueForField(tosend, 'name', 'contains', event.filter);
    setFilter(JSON.stringify(tosend));
  }

  return (
    <div className="iconSelector flex align-contents-center w-full min-w-full min-w-10rem" >
      <Dropdown
        className={className}
        disabled={props.disabled}
        filter
        filterBy="name"
        filterInputAutoFocus
        itemTemplate={iconOptionTemplate}
        onChange={onChange}
        onFilter={onFilter}
        optionLabel="name"
        optionValue="source"
        options={dataSource}
        placeholder="Select an Icon"
        scrollHeight="40vh"
        style={{
          ...{
            backgroundColor: 'var(--mask-bg)',
            overflow: 'hidden',
            textOverflow: 'ellipsis',
            whiteSpace: 'nowrap',
          },
        }}
        value={selectedIcon}
        valueTemplate={selectedTemplate}
        virtualScrollerOptions={{
          delay: 200,
          itemSize: 78,
          lazy: true,
          loadingTemplate: loadingTemplate,
          // eslint-disable-next-line @typescript-eslint/no-explicit-any
          onLazyLoad: (e: any) => {
            if (e.filter === '' && e.last as number >= index) {
              console.debug(index, e, (e.last as number + 100));
              let firstRecord = e.first as number < index ? index : e.first as number;
              setFirst(firstRecord);
              setLast((e.last as number + 100));
            }
          },

          showLoader: false,

        }}

      />
    </div>
  );
};

IconSelector.displayName = 'IconSelector';
IconSelector.defaultProps = {
  className: null,
  disabled: false,
  enableEditMode: true,
  loading: false,
  useDefault: true
};

type IconSelectorProps = {
  className?: string | null;
  disabled?: boolean;
  enableEditMode?: boolean;
  loading?: boolean;
  onChange: ((value: string) => void);
  useDefault?: boolean;
  value: string;
};

export default React.memo(IconSelector);
