const fs = require('fs');
const path = require('path');

const filesToPatch = [
    '../node_modules/@volo/ngx-lepton-x.core/fesm2022/volo-ngx-lepton-x.core.mjs',
    '../node_modules/@volo/abp.ng.lepton-x.core/fesm2022/volo-abp.ng.lepton-x.core.mjs'
];

filesToPatch.forEach(relativePath => {
    const filePath = path.join(__dirname, relativePath);

    if (!fs.existsSync(filePath)) {
        console.log(`File not found: ${relativePath}, skipping.`);
        return;
    }

    console.log(`Patching ${relativePath}...`);
    let content = fs.readFileSync(filePath, 'utf8');
    let modified = false;

    // 1. Remove DOCUMENT from @angular/core import
    const coreImportRegex = /(import \{[^}]*?)(\bDOCUMENT\b,?\s*)([^}]*?\} from '@angular\/core';)/;
    if (coreImportRegex.test(content)) {
        console.log('  Removing DOCUMENT from @angular/core import...');
        content = content.replace(coreImportRegex, (match, p1, p2, p3) => {
            // Remove DOCUMENT and potential trailing comma/space
            // simple approach: replace the group 2 with empty string, but check commas
            return p1 + p3;
        });
        // Cleanup double commas if any lefovers
        content = content.replace(/,\s*,/g, ',');
        modified = true;
    } else {
        // Fallback for string match if regex fails due to line breaks etc (though fesm is usually one line or standard fmt)
        if (content.includes('DOCUMENT, ') && content.includes("from '@angular/core'")) {
            console.log('  Removing DOCUMENT via string replace...');
            content = content.replace('DOCUMENT, ', '');
            modified = true;
        }
    }

    // 2. Ensure DOCUMENT is imported from @angular/common
    if (content.includes("from '@angular/common'")) {
        // Common import exists
        if (content.includes('DOCUMENT as DOCUMENT$1')) {
            console.log('  Replacing DOCUMENT as DOCUMENT$1 with DOCUMENT...');
            content = content.replace('DOCUMENT as DOCUMENT$1', 'DOCUMENT');
            modified = true;
        } else if (!content.includes('DOCUMENT') && !content.includes('DOCUMENT,')) {
            // Does not import DOCUMENT yet (and we just removed it from core, so we need it here)
            // Check if we need to add it to common list
            // This is tricky with regex. 
            // Safer: Replace "from '@angular/common';" with ", DOCUMENT } from '@angular/common';"
            // ONLY if we actually utilize DOCUMENT in the file (which we assume we do if we removed it from core)
            console.log('  Adding DOCUMENT to @angular/common import...');
            content = content.replace("} from '@angular/common';", ", DOCUMENT } from '@angular/common';");
            modified = true;
        }
    } else {
        // Common import does NOT exist. Add it.
        console.log('  Adding @angular/common import with DOCUMENT...');
        // Add it after core import
        content = content.replace("';\n", "';\nimport { DOCUMENT } from '@angular/common';\n");
        modified = true;
    }

    // 3. Replace usage of DOCUMENT$1 with DOCUMENT
    if (content.includes('DOCUMENT$1')) {
        console.log('  Replacing usages of DOCUMENT$1...');
        content = content.replace(/DOCUMENT\$1/g, 'DOCUMENT');
        modified = true;
    }

    // 4. Cleanup any mess created in imports (like ", ,")
    content = content.replace(/{ ,/g, '{');
    content = content.replace(/, }/g, ' }');


    if (modified) {
        fs.writeFileSync(filePath, content, 'utf8');
        console.log(`  Successfully patched ${relativePath}`);
    } else {
        console.log(`  No changes needed for ${relativePath}`);
    }
});
