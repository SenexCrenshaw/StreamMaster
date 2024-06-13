type ExcludeIfChildrenSet<T> = T extends { children: React.ReactNode }
  ? Omit<T, 'data' | 'dataKey' | 'filter' | 'filterBy' | 'itemTemplate' | 'itemSize' | 'onChange' | 'optionValue' | 'selectedItemsKey' | 'select' | 'value'>
  : T;

interface SMScrollerConditionalProperties {
  readonly className?: string;
  readonly data?: any;
  readonly dataKey?: string;
  readonly filter?: boolean;
  readonly filterBy?: string;
  readonly itemSize?: number;
  readonly itemTemplate?: (item: any) => React.ReactNode;
  readonly onChange?: (value: any) => void;
  readonly optionValue?: string;
  readonly scrollHeight?: string;
  readonly select?: boolean;
  readonly selectedItemsKey?: string;
  readonly simple?: boolean;
  readonly value?: any;
}
export type SMScrollerProperties = ExcludeIfChildrenSet<SMScrollerConditionalProperties>;
