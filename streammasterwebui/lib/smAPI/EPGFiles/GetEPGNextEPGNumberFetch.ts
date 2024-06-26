import { GetEPGNextEPGNumber } from '@lib/smAPI/EPGFiles/EPGFilesCommands';
import { createAsyncThunk } from '@reduxjs/toolkit';
import { Logger } from '@lib/common/logger';


export const fetchGetEPGNextEPGNumber = createAsyncThunk('cache/getGetEPGNextEPGNumber', async (_: void, thunkAPI) => {
  try {
    Logger.debug('Fetching GetEPGNextEPGNumber');
    const response = await GetEPGNextEPGNumber();
    return {param: _, value: response };
  } catch (error) {
    console.error('Failed to fetch', error);
    return thunkAPI.rejectWithValue({ error: error || 'Unknown error', value: undefined });
  }
});


