import StringEditorBodyTemplate from '@components/inputs/StringEditorBodyTemplate';
import { getIconUrl } from '@lib/common/common';
import useElementSize from '@lib/hooks/useElementSize';
import useGetIcons from '@lib/smAPI/Icons/useGetIcons';
import { IconFileDto } from '@lib/smAPI/smapiTypes';
import { Button } from 'primereact/button';
import { DataView, DataViewLayoutOptions } from 'primereact/dataview';
import { useLocalStorage } from 'primereact/hooks';
import { classNames } from 'primereact/utils';
import { useCallback, useEffect, useRef, useState } from 'react';

type IconSelectorProperties = {
  readonly onClick: (value: string) => void;
  readonly iconSource: string | undefined;
};

const IconGrid = ({ iconSource, onClick }: IconSelectorProperties) => {
  const icons = useGetIcons();
  const [origIconSource, setOrigIconSource] = useState<string | undefined>(undefined);
  const [first, setFirst] = useState<number>(0);
  const [filter, setFilter] = useState<string | undefined>(undefined);
  const [layout, setLayout] = useState<'list' | 'grid' | (string & Record<string, unknown>)>('grid');
  // const [isLoading, setIsLoading] = useState<boolean>(false);
  const [rows, setRows] = useLocalStorage<number>(50, 'IconGrid-rows');
  // const [currentPage, setCurrentPage] = useLocalStorage<number>(1, 'IconGrid-currentPage');
  const [dataSource, setDataSource] = useState<IconFileDto[] | undefined>([]);

  const parentRef = useRef<HTMLDivElement>(null);
  const { width: elementWidth } = useElementSize(parentRef);

  useEffect(() => {
    if (elementWidth) {
      const howManyItems = Math.floor(elementWidth / 23);
      setRows(howManyItems);
    }
  }, [elementWidth, setRows]);

  const listItem = (icon: IconFileDto, index: number) => {
    return (
      <div className="col-12" key={icon.Id}>
        <div className={classNames('flex flex-column xl:flex-row xl:align-items-start p-4 gap-4', { 'border-top-1 surface-border': index !== 0 })}>
          <img className="w-9 sm:w-16rem xl:w-10rem shadow-2 block xl:block mx-auto border-round" src={icon.Source} alt={icon.Name} />
          <div className="flex flex-column sm:flex-row justify-content-between align-items-center xl:align-items-start flex-1 gap-4">
            <div className="flex flex-column align-items-center sm:align-items-start gap-3">
              <div className="text-2xl font-bold text-900">{icon.Name}</div>

              <div className="flex align-items-center gap-3">
                <span className="flex align-items-center gap-2">
                  <i className="pi pi-tag"></i>
                  <span className="font-semibold">{icon.Name}</span>
                </span>
              </div>
            </div>
            <div className="flex sm:flex-column align-items-center sm:align-items-end gap-3 sm:gap-2">
              <span className="text-2xl font-semibold">${icon.Name}</span>
            </div>
          </div>
        </div>
      </div>
    );
  };

  const isSelected = useCallback(
    (icon: IconFileDto) => {
      return icon.Source === iconSource;
    },
    [iconSource]
  );

  const goToPage = useCallback(
    (page: number) => {
      const test = page ? rows * (page - 1) : 0;
      setFirst(test);
    },
    [rows]
  );

  const goToIcon = useCallback(
    (iconSource: string) => {
      const index = icons.data?.findIndex((icon) => icon.Source === iconSource);
      if (index && index > -1) {
        const page = Math.floor(index / rows) + 1;
        goToPage(page);
      }
    },
    [goToPage, icons.data, rows]
  );

  useEffect(() => {
    if (origIconSource === undefined && iconSource !== undefined && iconSource !== '') {
      setOrigIconSource(iconSource);
      goToIcon(iconSource);
    }
  }, [goToIcon, iconSource, origIconSource]);

  const gridItem = (icon: IconFileDto) => {
    const iconUrl = icon.Source ? getIconUrl(icon.Source, '', false) : '';

    return (
      <div className={`listTemplate-gridItem-container ${isSelected(icon) ? 'selected' : ''}`}>
        <Button
          className="button"
          key={icon.Id}
          onClick={() => {
            onClick(icon.Source);
          }}
        >
          <div>
            <div className="listTemplate-gridItem">
              <img
                className="img"
                src={iconUrl}
                alt={icon.Name}
                onError={(e) => {
                  (e.currentTarget as HTMLImageElement).src = '/images/default.png';
                }}
                loading="lazy"
              />
            </div>
            <div className="listTemplate-gridItem-name text-xs">{icon.Name}</div>
          </div>
        </Button>
      </div>
    );
  };

  const itemTemplate = (icon: IconFileDto, index: number) => {
    if (!icon) {
      return;
    }
    if (layout === 'list') return listItem(icon, index);
    else if (layout === 'grid') return gridItem(icon);
  };

  const listTemplate = (icons: IconFileDto[]) => {
    return <div className="grid grid-nogutter">{icons.map((icon, index) => itemTemplate(icon, index))}</div>;
  };

  useEffect(() => {
    if (!icons.data) {
      return;
    }
    if (filter === undefined || filter === '') {
      setDataSource(icons.data);
      return;
    }
    const data = icons.data.filter(
      (icon) => icon.Name.toLowerCase().includes(filter.toLowerCase()) || icon.Source.toLowerCase().includes(filter.toLowerCase())
    );
    setDataSource(data);
  }, [filter, icons.data]);

  const header = () => {
    return (
      <div className="flex justify-content-between align-items-center w-full p-0 m-0">
        <div className="w-6">
          <StringEditorBodyTemplate
            value={iconSource}
            onChange={(value) => {
              if (value) {
                onClick(value);
                console.log(value);
              }
            }}
          />
        </div>
        <div className="flex justify-content-end align-items-center  col-6 p-0 m-0">
          <StringEditorBodyTemplate
            autofocus
            placeholder="Search"
            isSearch={true}
            value={filter}
            onChange={(value) => {
              setFilter(value);
              goToPage(1);
            }}
            onFilterClear={() => {
              setFilter('');
            }}
          />
          <DataViewLayoutOptions
            layout={layout}
            onChange={(e) => {
              setLayout(e.value);
            }}
          />
        </div>
      </div>
    );
  };

  // const pagingTemplate = {
  //   layout: 'RowsPerPageDropdown FirstPageLink PrevPageLink JumpToPageInput PageLinks NextPageLink LastPageLink CurrentPageReport'
  // };
  // const pagingTemplate = {
  //   layout: 'RowsPerPageDropdown FirstPageLink PrevPageLink PageLinks NextPageLink LastPageLink CurrentPageReport',
  //   RowsPerPageDropdown: (options: PaginatorRowsPerPageDropdownOptions) => {
  //     const dropdownOptions = [
  //       { label: 25, value: 25 },
  //       { label: 50, value: 50 },
  //       { label: 75, value: 75 },
  //       { label: 100, value: 100 },
  //       { label: 200, value: 200 }
  //     ];

  //     return (
  //       <React.Fragment>
  //         <span className="mx-1" style={{ color: 'var(--text-color)', userSelect: 'none' }}>
  //           Logos per page:{' '}
  //         </span>
  //         <Dropdown
  //           value={rows}
  //           options={dropdownOptions}
  //           onChange={(e) => {
  //             setRows(e.value);
  //             options.onChange(e);
  //           }}
  //         />
  //       </React.Fragment>
  //     );
  //   }
  // };

  if (icons.isLoading) {
    return <div>Loading...</div>;
  }

  return (
    <div ref={parentRef} className="w-full h-full ">
      <DataView
        first={first}
        value={dataSource ?? undefined}
        // paginatorTemplate={pagingTemplate}
        listTemplate={listTemplate}
        header={header()}
        paginator
        rows={rows}
        onPage={(e) => {
          goToPage(e.page + 1);
        }}
      />
    </div>
  );
};

export default IconGrid;
